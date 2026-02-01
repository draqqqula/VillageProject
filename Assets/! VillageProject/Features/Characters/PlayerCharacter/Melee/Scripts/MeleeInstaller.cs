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
    [SerializeField] private GuardConfiguration _guardConfiguration;
    [SerializeField] private ParryingConfiguration parryingConfiguration;
    
    [Header("Dependency")]
    [SerializeField] private InputActionReference _attack;
    [SerializeField] private InputActionReference _guard;
    [SerializeField] private Animator _animator;
    [SerializeField] private CoroutineHandler _coroutineHandler;
    [SerializeField] private HitboxEvent _shieldHitboxEvent;
    [SerializeField] private ParryingUpdater parryingUpdater;

    private MeleeControlsPresetManager _preset;
    
    [SerializeField] private InputActionReference _leftAction;
    [SerializeField] private InputActionReference _rightAction;
    [SerializeField] private InputActionReference _blockAction;

    [Inject]
    private void Construct(MeleeControlsPresetManager presetManager)
    {
        _preset = presetManager;
    }
    
    public override void InstallBindings()
    {
        Container.BindInstance(_animator);
        Container.BindInstance(_coroutineHandler);
        Container.BindInstance(_shieldHitboxEvent).AsSingle();

        Container.BindInstance(_swingConfiguration).AsSingle();
        Container.BindInstance(_slashConfiguration).AsSingle();
        Container.BindInstance(_guardConfiguration).AsSingle();
        Container.BindInstance(parryingConfiguration).AsSingle();
        
        Container.Bind<CursorDeltaHandler>().AsTransient().OnInstantiated<CursorDeltaHandler>((context, it) => it.Initialize());
        Container.Bind<InterruptToGuardTransitionChecker>().AsTransient().OnInstantiated<InterruptToGuardTransitionChecker>((context, it) => it.Initialize());
        Container.BindInterfacesAndSelfTo<SlashSeriesCounter>().AsSingle();
        Container.BindInterfacesAndSelfTo<AttackBlendingController>().AsSingle();
        Container.BindInstance(parryingUpdater).AsSingle();
        
        Container.BindInterfacesAndSelfTo<AttackInput>().FromInstance(new AttackInput(_attack, _preset)).AsSingle();
        Container.BindInterfacesAndSelfTo<BlockInput>().FromInstance(new BlockInput(_guard, _preset)).AsSingle();
        
        
        Container.BindInterfacesAndSelfTo<ShiftInput>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<IdleState>().AsTransient();
        Container.Bind<IState>().WithId(StateManager.DefaultStateId).To<IdleState>().FromResolve().AsTransient();
        Container.BindInterfacesAndSelfTo<SlashState>().AsTransient();
        Container.BindInterfacesAndSelfTo<SwingState>().AsTransient();
        Container.BindInterfacesAndSelfTo<ThrustState>().AsTransient();
        Container.Bind<GuardState>().AsTransient();
        
        Container.Bind<TransitionBase<IdleState>>().To<IdleToSwingTransition>().AsTransient();
        Container.Bind<TransitionBase<SwingState>>().To<SwingToSlashTransition>().AsTransient();
        //Container.Bind<TransitionBase<SwingState>>().To<SwingToThrustTransition>().AsTransient();
        Container.Bind<TransitionBase<SlashState>>().To<SlashToSwingTransition>().AsTransient();
        Container.Bind<TransitionBase<SlashState>>().To<EndingToIdleTransition<SlashState>>().AsTransient();
        Container.Bind<TransitionBase<ThrustState>>().To<EndingToIdleTransition<ThrustState>>().AsTransient();
        Container.Bind<TransitionBase<IdleState>>().To<IdleToGuardTransition>().AsTransient();
        Container.Bind<TransitionBase<GuardState>>().To<GuardToIdleTransition>().AsTransient();
        Container.Bind<TransitionBase<GuardState>>().To<GuardToIdleBreakingTransition>().AsTransient();
        Container.Bind<TransitionBase<GuardState>>().To<GuardToSwingTransition>().AsTransient();
        Container.Bind<TransitionBase<SwingState>>().To<AnyToGuardTransitionA<SwingState>>().AsTransient();
        Container.Bind<TransitionBase<SlashState>>().To<AnyToGuardTransitionB>().AsTransient();
        
        Container.BindInterfacesAndSelfTo<StateManager>().AsSingle();

        var leftAttack = new InputWithHolding(_leftAction);
        var rightAttack = new InputWithHolding(_rightAction);
        var holdingAction = new InputWithHolding(_blockAction);
        leftAttack.Initialize();
        rightAttack.Initialize();
        holdingAction.Initialize();

        Container.BindInstance(new MouseButtonsControlHandler(leftAttack, rightAttack, holdingAction, _coroutineHandler )).AsSingle();
        //Container.BindInterfacesAndSelfTo<MouseButtonsControlHandler>().AsSingle().WithArguments(new object[] { leftAttack, rightAttack, holdingAction });
    }
}