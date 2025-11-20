using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class SlashSeriesCounter
{
    private AttackDirection _nextDirection;

    public void SetNextAttack(AttackDirection direction)
    {
        switch (direction)
        {
            case AttackDirection.LeftSwing:
                _nextDirection = AttackDirection.RightSwing; break;
            case AttackDirection.RightSwing:
                _nextDirection = AttackDirection.LeftSwing; break;
        }
    }

    public AttackDirection GetDirection()
    {
        return _nextDirection;
    }
}