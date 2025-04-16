using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatController : InputListener
{
    private const string AttackTrigger = "Attack";
    private const string AttackTag = "Attack";
    private const string SeriesVariable = "Series";

    [SerializeField, FromInputActionAsset("Attack")] public InputActionReference Attack;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _maxSeries = 1;
    [SerializeField] private float _noInterruptionWindow = 0.7f;
    [SerializeField] private float _noScheduleWindow = 0.3f;
    private int _successiveCounter = 0;
    private Coroutine _scheduledAttack;

    private float _cachedTime = 0;

    private void OnEnable()
    {
        Attack.action.performed += HandleAttackInput;
    }

    private void OnDisable()
    {
        Attack.action.performed -= HandleAttackInput;
    }

    private void HandleAttackInput(InputAction.CallbackContext context)
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
        //Debug.Log($"Attack scheduled for {remainingTime} s.");
        yield return new WaitForSeconds(remainingTime);
        StartAttack();
    }

    private void StartAttack()
    {
        Debug.Log($"Attack started after {Time.time - _cachedTime}");
        if (Time.time - _cachedTime < _noInterruptionWindow * 0.833)
        {
            Debug.Log("Early attack");
        }
        _cachedTime = Time.time;
        SetNextAttack();
        _animator.SetTrigger(AttackTrigger);
    }
}
