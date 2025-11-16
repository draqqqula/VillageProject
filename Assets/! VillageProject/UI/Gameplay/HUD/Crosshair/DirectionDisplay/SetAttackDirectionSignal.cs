using System.Collections;
using UnityEngine;

public class SetAttackDirectionSignal{
    public SetAttackDirectionSignal(AttackDirection direction)
    {
        Direction = direction;
    }
    public AttackDirection Direction { get; private set; }
}