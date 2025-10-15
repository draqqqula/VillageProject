using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using Zenject;

public class MeleeInstaller : MonoInstaller
{
    [Header("Configuration")]
    [SerializeField] private SwingConfiguration _swingConfiguration;
    [SerializeField] private SlashConfiguration _slashConfiguration;
    [Header("Dependency")]
    [SerializeField] private InputActionReference _attack;
    [SerializeField] private Animator _animator;
    [SerializeField] private CoroutineHandler _coroutineHandler;

    public override void InstallBindings()
    {
        Container.BindInstance(_animator);
        Container.BindInstance(_coroutineHandler);
        Container.BindInstance(_swingConfiguration).AsSingle();
        Container.BindInstance(_slashConfiguration).AsSingle();
        Container.BindInterfacesAndSelfTo<SlashSeriesCounter>().AsSingle();
        Container.BindInterfacesAndSelfTo<AttackInput>().FromInstance(new AttackInput(_attack)).AsSingle();
        Container.BindInterfacesAndSelfTo<IdleState>().AsTransient();
        Container.BindInterfacesAndSelfTo<SlashState>().AsTransient();
        Container.BindInterfacesAndSelfTo<SwingState>().AsTransient();
        Container.BindInterfacesAndSelfTo<ThrustState>().AsTransient();
        Container.Bind<TransitionBase<IdleState>>().To<IdleToSwingTransition>().AsTransient();
        Container.Bind<TransitionBase<SwingState>>().To<SwingToSlashTransition>().AsTransient();
        Container.Bind<TransitionBase<SwingState>>().To<SwingToThrustTransition>().AsTransient();
        Container.Bind<TransitionBase<SlashState>>().To<EndingToSwingTransitionA<SlashState>>().AsTransient();
        Container.Bind<TransitionBase<SlashState>>().To<EndingToIdleTransition<SlashState>>().AsTransient();
        Container.Bind<TransitionBase<ThrustState>>().To<EndingToSwingTransitionB<ThrustState>>().AsTransient();
        Container.Bind<TransitionBase<ThrustState>>().To<EndingToIdleTransition<ThrustState>>().AsTransient();
        Container.BindInterfacesAndSelfTo<StateManager>().AsSingle();
    }
}