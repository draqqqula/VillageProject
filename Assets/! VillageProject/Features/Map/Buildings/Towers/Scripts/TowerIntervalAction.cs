using System;
using System.Collections;
using UnityEngine;

public abstract class TowerIntervalAction : MonoBehaviour
{

    [SerializeField] protected TowerRange Range;
    [SerializeField] private float _animDelay;
    private Coroutine _coroutine;

    public event Action OnAnimInvoked;

    private void OnEnable()
    {
        Range.OnTargetEnter += HandleTargetEnter;
    }

    private void OnDisable()
    {
        Range.OnTargetEnter -= HandleTargetEnter;
    }

    private void HandleTargetEnter(Health health)
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine(PerformOverTime());
        }
    }

    private IEnumerator PerformOverTime()
    {
        while (true)
        {
            if (Range.Targets.Count != 0)
            {
                OnAnimInvoked?.Invoke();
                yield return new WaitForSeconds(_animDelay);
                yield return new WaitForSeconds(Perform() - _animDelay);
            }
            else
            {
                _coroutine = null;
                HandleBreak();
                yield break;
            }
        }
    }

    protected abstract float Perform();

    protected virtual void HandleBreak()
    {

    }
}