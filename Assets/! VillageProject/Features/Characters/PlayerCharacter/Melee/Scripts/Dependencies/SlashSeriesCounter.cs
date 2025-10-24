using System.Collections;
using UnityEngine;
using Zenject;

public class SlashSeriesCounter
{
    private const float MaxSeries = 1;
    private const string SeriesVariable = "Series";

    private int _successiveCounter = 0;
    [Inject] private Animator _animator;

    public int SuccessiveCounter => _successiveCounter;

    public void SetNextAttack()
    {
        if (_successiveCounter < MaxSeries)
        {
            _successiveCounter++;
        }
        else
        {
            _successiveCounter = 0;
        }
        _animator.SetInteger(SeriesVariable, _successiveCounter);
    }
}