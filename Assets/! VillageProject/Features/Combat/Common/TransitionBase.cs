using R3;
using System.Collections;
using UnityEngine;
using Zenject;

public abstract class TransitionBase<T> : ITransition where T : IState
{
    private Subject<Unit> _onActivated = new Subject<Unit>();
    public Observable<Unit> OnActivated => _onActivated;

    protected void Activate()
    {
        _onActivated.OnNext(Unit.Default);
    }

    public abstract void Construct(T currentState);
    public abstract IState GetNextState();
}

public abstract class TransitionBase<A, B> : TransitionBase<A> where A : IState
{
    [Inject] private DiContainer _container;

    public override IState GetNextState()
    {
        return (IState)_container.Resolve<B>();
    }
}