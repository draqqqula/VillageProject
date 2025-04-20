using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : InputListener
{
    private const string AttackTrigger = "Attack";
    private const string AttackTag = "Attack";
    private const string SeriesVariable = "Series";
    private const string ThrustTrigger = "Thrust";

    [SerializeField, FromInputActionAsset("Attack")] public InputActionReference Attack;
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

    private void OnEnable()
    {
        Attack.action.started += HandleAttackInputPressed;
        Attack.action.canceled += HandleAttackInputReleased;
    }

    private void OnDisable()
    {
        Attack.action.started -= HandleAttackInputPressed;
        Attack.action.canceled -= HandleAttackInputReleased;
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
            StopCoroutine(_holdingCoroutine);
            _holdingCoroutine = null;
        }
        _animator.speed = 1;
    }

    public void HandleAttackAnimationStarted()
    {
        _holdingCoroutine = StartCoroutine(DelayEnterHolding());
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

    private IEnumerator DelayEnterHolding()
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
                _animator.SetTrigger(ThrustTrigger);
                break;
            }

            yield return new WaitForFixedUpdate();
            holdDuration += Time.fixedDeltaTime;
        }
        _animator.speed = 1;
    }

    private void StartAttack()
    {
        SetNextAttack();
        _animator.SetTrigger(AttackTrigger);
    }
}
