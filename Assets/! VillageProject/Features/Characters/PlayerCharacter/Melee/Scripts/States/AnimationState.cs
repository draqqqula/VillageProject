using System.Collections;
using UnityEngine;
using Zenject;
using R3;

public abstract class AnimationState<T> : EndingState<T> where T : AnimationState<T>
{
    protected IAnimationWindowListener _listener;

    protected abstract AnimationWindow Window { get; }

    [Inject]
    private void Construct(DiContainer container)
    {
        _listener = container.ResolveId<IAnimationWindowListener>(Window);
    }

    private void HandleAnimationEnded()
    {
        _hasEnded.Value = true;
    }

    public override void OnEnter()
    {
        _listener.OnExit += HandleAnimationEnded;
    }

    public override void OnExit()
    {
        _listener.OnExit -= HandleAnimationEnded;
    }
}