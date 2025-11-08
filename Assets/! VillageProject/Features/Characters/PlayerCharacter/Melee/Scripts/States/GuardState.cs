using R3;
using UnityEngine;
using Zenject;

public sealed class GuardState : AnimationState<GuardState>
{
    private const string BlockParam = "Block";
    private const string HitParam = "Hit";
    
    public override StateType StateType => StateType.Guard;
    [Inject] private GuardConfiguration _guardConfiguration;
    protected override AnimationWindow Window => _guardConfiguration.Window;

    [Inject(Id = "AddGuardHitbox")] private IAnimationWindowListener _addHitboxWindowListener;
    [Inject(Id = "RemoveGuardHitbox")] private IAnimationWindowListener _removeHitboxWindowListener;

    [Inject] private Animator _animator;
    [Inject] private Stamina _stamina;
    
    [Inject] private HitboxEvent _shieldHitboxEvent;
    private HitRegistrar _registrar;
    
    public override void OnEnter()
    {
        _registrar = new HitboxHitRegistrar(_shieldHitboxEvent);
        _registrar.Deactivate();
        
        _addHitboxWindowListener.OnEnter += OnAddingHitboxWindow;
        _removeHitboxWindowListener.OnEnter += OnRemovingHitboxWindow;
        _registrar.Activate();
        
        _stamina.ModifyRate(_guardConfiguration.StaminaFillModifier).AddTo(this);
        _animator.SetBool(BlockParam, true);
    }

    public override void OnExit()
    {
        _addHitboxWindowListener.OnEnter -= OnAddingHitboxWindow;
        _animator.SetBool(BlockParam, false);
    }

    private void OnAddingHitboxWindow()
    {
        _registrar.OnHit += OnHit;
    }

    private void OnRemovingHitboxWindow()
    {
        _registrar.OnHit -= OnHit;
        _removeHitboxWindowListener.OnEnter -= OnRemovingHitboxWindow;
        
        _registrar.Deactivate();
        _registrar.Dispose();
    }
    
    private void OnHit()
    {
        _animator.SetTrigger(HitParam);
    }
}