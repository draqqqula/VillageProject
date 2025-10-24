using System.Collections;
using UnityEngine;
using R3;
using System;
using Zenject;

public class EndingToIdleTransition<T> : TransitionBase<T> where T : EndingState<T>
{
    [Inject(Id = "Idle")] private IAnimationWindowListener _idleWindow;

    public override void Construct(T currentState)
    {
        var subscription = currentState.HasEnded.Subscribe(HandleEnded).AddTo(this);
    }

    private void HandleEnded(bool value)
    {
        if (value)
        {
            _idleWindow.OnEnter += HandleIdleStarted;
        }
    }

    private void HandleIdleStarted()
    {
        Activate<IdleState>();
        _idleWindow.OnEnter -= HandleIdleStarted;
    }

    public override void Dispose()
    {
        _idleWindow.OnEnter -= HandleIdleStarted;
        base.Dispose();
    }
}