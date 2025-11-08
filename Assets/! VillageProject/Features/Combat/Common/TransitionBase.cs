using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public abstract class TransitionBase<T> : CompositeDisposableBase, ITransition where T : IState
{
    [Inject] private DiContainer _container;
    private Subject<IState> _onActivated = new Subject<IState>();
    public Observable<IState> OnActivated => _onActivated;

    protected void Activate<TNext>()
    {
        var state = (IState)_container.Resolve<TNext>();
        _onActivated.OnNext(state);
    }

    public abstract void Construct(T currentState);
}

public abstract class TransitionBase<A, B> : TransitionBase<A> where A : IState
{
    protected void Activate()
    {
        Activate<B>();
    }
}