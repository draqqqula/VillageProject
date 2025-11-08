using System.Collections;
using UnityEngine;

public class IdleState : StateBase<IdleState>
{
    public override StateType StateType => StateType.Idle;

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }
}