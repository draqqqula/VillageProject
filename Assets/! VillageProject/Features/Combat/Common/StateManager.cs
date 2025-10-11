using System.Collections;
using UnityEngine;
using Zenject;
using R3;
using System.Collections.Generic;

public class StateManager : IInitializable
{
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

        public void Activate(Unit _)
        {
            _stateManager.SetState(_transition.GetNextState());
            _subsriptions.Dispose();
        }
    }

    [Inject] private DiContainer _container;
    private ReactiveProperty<IState> _current = new ReactiveProperty<IState>();
    
    public ReadOnlyReactiveProperty<IState> Current => _current;

    public void Initialize()
    {
        var initial = _container.Resolve<IState>();
        SetState(initial);
    }

    private void SetState(IState state)
    {
        _current.Value?.OnExit();
        _current.Value = state;
        state.OnEnter();

        var transitions = state.GetTransitions();
        var subscriptions = new CompositeDisposable();

        foreach (var transition in transitions)
        {
            var handler = new TransitionHandler(subscriptions, transition, this);
            transition.OnActivated.Subscribe(handler.Activate).AddTo(subscriptions);
        }
    }
}