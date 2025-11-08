using System.Collections;
using UnityEngine;
using Zenject;
using R3;
using System.Collections.Generic;
using System.Linq;

public class StateManager : IInitializable
{
    public const string DefaultStateId = "Default";

    class TransitionHandler
    {
        private CompositeDisposable _subsriptions;
        private ITransition _transition;
        private StateManager _stateManager;

        public TransitionHandler(CompositeDisposable subscriptions, ITransition transition, StateManager manager)
        {
            _stateManager = manager;
            _subsriptions = subscriptions;
            _transition = transition;
        }

        public void Activate(IState next)
        {
            _stateManager.SetState(next);
            _subsriptions.Dispose();
        }
    }

    [Inject] private DiContainer _container;
    private ReactiveProperty<IState> _current = new ReactiveProperty<IState>();
    
    public ReadOnlyReactiveProperty<IState> Current => _current;

    public void Initialize()
    {
        var initial = _container.ResolveId<IState>(DefaultStateId);
        SetState(initial);
    }

    private void SetState(IState state)
    {
        _current.Value?.OnExit();
        _current.Value?.Dispose();
        _current.Value = state;
        state.OnEnter();

        var transitions = state.GetTransitions();
        var subscriptions = new CompositeDisposable();

        foreach (var transition in transitions)
        {
            var handler = new TransitionHandler(subscriptions, transition, this);
            subscriptions.Add(transition.OnActivated.Subscribe(handler.Activate));
            subscriptions.Add(transition);
        }
    }
}