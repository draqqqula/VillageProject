using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : InputListener
{
    private const string AttackTrigger = "Attack";
    private const string AttackTag = "Attack";
    private const string SeriesVariable = "Series";
    private const string ThrustTrigger = "Thrust";

    [SerializeField] private Stamina _stamina;
    private IDisposable _rateModifier;

    [SerializeField, FromInputActionAsset("Attack")] public InputActionReference Attack;
    [SerializeField] private AnimationWindow _slashWindow;
    [SerializeField] private AnimationWindow _thrustWindow;
    private AnimationWindowListener _slashListener;
    private AnimationWindowListener _thrustListener;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _maxSeries = 1;
    [SerializeField] private float _noInterruptionWindow = 0.7f;
    [SerializeField] private float _noScheduleWindow = 0.3f;

    [Header("Holding")]
    [SerializeField] private float _stamp = 0.2f;
    [SerializeField] private float _slowdownEnterTime = 0.1f;
    [SerializeField] private float _slowdownExitTime = 1f;
    [SerializeField] private float _slowdownEnterFactor = 1f;
    [SerializeField] private float _slowdownExitFactor = 1f;
    [SerializeField] private float _maxSlowdown = 1f;
    [SerializeField] private float _minSlowdown = 0.8f;

    private int _successiveCounter = 0;
    private Coroutine _scheduledAttack;
    private Coroutine _holdingCoroutine;

    private void Awake()
    {
        var listeners = GetComponentsInChildren<AnimationWindowListener>();
        _slashListener = listeners.First(it => it.Window.Equals(_slashWindow));
        _thrustListener = listeners.First(it => it.Window.Equals(_thrustWindow));
    }

    private void OnEnable()
    {
        Attack.action.started += HandleAttackInputPressed;
        Attack.action.canceled += HandleAttackInputReleased;
        _slashListener.OnEntered += HandleSlashStarted;
        _thrustListener.OnEntered += HandleThrustStarted;

        _slashListener.OnExited += HandleSlashEnded;
        _thrustListener.OnExited += HandleThrustEnded;
    }

    private void OnDisable()
    {
        Attack.action.started -= HandleAttackInputPressed;
        Attack.action.canceled -= HandleAttackInputReleased;
        _slashListener.OnEntered -= HandleSlashStarted;
        _thrustListener.OnEntered -= HandleThrustStarted;

        _slashListener.OnExited -= HandleSlashEnded;
        _thrustListener.OnExited -= HandleThrustEnded;
    }

    private void HandleAttackInputPressed(InputAction.CallbackContext context)
    {
        var animatorState = _animator.GetCurrentAnimatorStateInfo(0);

        if (animatorState.IsTag(AttackTag)
            && animatorState.normalizedTime <= _noInterruptionWindow)
        {
            if (animatorState.normalizedTime > _noScheduleWindow)
            {
                if (_scheduledAttack != null)
                {
                    StopCoroutine(_scheduledAttack);
                }
                _scheduledAttack = StartCoroutine(ScheduleAttack(animatorState));
            }
            return;
        }

        StartAttack();
    }

    private void HandleAttackInputReleased(InputAction.CallbackContext context)
    {
        if (_holdingCoroutine != null)
        {
            _stamina.TrySpend(3);
            StopCoroutine(_holdingCoroutine);
            _holdingCoroutine = null;
        }
        _animator.speed = 1;
    }

    public void HandleSlashStarted()
    {
        _holdingCoroutine = StartCoroutine(DelayEnterHolding(true));
        _rateModifier = _stamina.ModifyRate(0);
    }

    public void HandleThrustStarted()
    {
        _holdingCoroutine = StartCoroutine(DelayEnterHolding(false));
        _rateModifier = _stamina.ModifyRate(0);
    }

    public void HandleSlashEnded()
    {
        _rateModifier.Dispose();
        _rateModifier = null;
    }

    public void HandleThrustEnded()
    {
        _rateModifier.Dispose();
        _rateModifier = null;
    }

    public void HandleAttackAnimationEnded()
    {

    }

    private void SetNextAttack()
    {
        if (_successiveCounter < _maxSeries)
        {
            _successiveCounter++;
        }
        else
        {
            _successiveCounter = 0;
        }
        _animator.SetInteger(SeriesVariable, _successiveCounter);
    }

    private IEnumerator ScheduleAttack(AnimatorStateInfo animatorState)
    {
        var remainingTime = (_noInterruptionWindow - animatorState.normalizedTime)
            * animatorState.length;
        yield return new WaitForSeconds(remainingTime);
        StartAttack();
    }

    private IEnumerator DelayEnterHolding(bool transistionToThrust)
    {
        yield return new WaitForSeconds(_stamp);

        var holdDuration = 0.0f;
        while (Attack.action.inProgress)
        {
            if (holdDuration < _slowdownEnterTime)
            {
                _animator.speed = Mathf.Pow(1 - Mathf.Clamp01(holdDuration / _slowdownEnterTime), _slowdownEnterFactor)
                * _maxSlowdown + (1 - _maxSlowdown);
            }
            else if (holdDuration < _slowdownEnterTime + _slowdownExitTime)
            {
                var exitDuration = holdDuration - _slowdownEnterTime;
                _animator.speed = Mathf.Pow(1 - Mathf.Clamp01(exitDuration / _slowdownExitTime), _slowdownEnterFactor)
                * (_minSlowdown - _maxSlowdown) - _minSlowdown + 1;
            }
            else
            {
                if (transistionToThrust)
                {
                    _animator.SetTrigger(ThrustTrigger);
                    _animator.speed = 1;
                    _holdingCoroutine = null;
                    yield break;
                }
                break;
            }

            yield return new WaitForFixedUpdate();
            holdDuration += Time.fixedDeltaTime;
        }
            _stamina.TrySpend(3);
        _animator.speed = 1;
        _holdingCoroutine = null;
    }

    private void StartAttack()
    {
        if (_stamina.IsOnCooldown.CurrentValue)
        {
            return;
        }
        SetNextAttack();
        _animator.SetTrigger(AttackTrigger);
    }
}
