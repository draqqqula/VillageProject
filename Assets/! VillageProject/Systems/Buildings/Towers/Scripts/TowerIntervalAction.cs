using System.Collections;
using UnityEngine;

public abstract class TowerIntervalAction : MonoBehaviour
{

    [SerializeField] protected TowerRange Range;
    private Coroutine _coroutine;

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
                yield return new WaitForSeconds(Perform());
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