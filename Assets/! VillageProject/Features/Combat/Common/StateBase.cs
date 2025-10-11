using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public abstract class StateBase<T> : IState where T : StateBase<T>
{
    [Inject] private DiContainer _container;

    public IEnumerable<ITransition> GetTransitions()
    {
        var transitions = _container.ResolveAll<TransitionBase<T>>();
        foreach (var transition in transitions)
        {
            transition.CurrentState = (T)this;
            yield return transition;
        }
    }

    public abstract void OnEnter();

    public abstract void OnExit();
}