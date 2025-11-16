using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class SlashSeriesCounter
{
    private const float MaxSeries = 1;

    private int _successiveCounter = 0;

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
    }

    public AttackDirection GetDirection()
    {
        return (AttackDirection)_successiveCounter;
    }
}