using R3;
using System;
using UnityEngine;
using Zenject;

public class InterruptToGuardTransitionChecker : CompositeDisposableBase
{
    [Inject(Id = "NotInterruptable")] private IAnimationWindowListener _notInterruptableWindow;
    [Inject] private IBlockInput _blockInput;
    [Inject] private Stamina _stamina;
    [Inject] private MeleeControlsPresetManager _controlsPreset;
    [Inject] private MouseButtonsControlHandler _mouseButtonsControlHandler;
    
    public ReactiveProperty<bool> CanInterrupt {get; private set;}

    public void Initialize()
    {
        CanInterrupt = new ReactiveProperty<bool>(false);
        _notInterruptableWindow.IsActive.Subscribe(TryInterrupt).AddTo(this);
        
       if (_controlsPreset.ChosenPreset.CurrentValue == 0) _blockInput.IsHolding.Subscribe(TryInterrupt).AddTo(this);
       else if (_controlsPreset.ChosenPreset.CurrentValue == 1) _mouseButtonsControlHandler.IsBlockHolding.Subscribe(TryInterrupt).AddTo(this);
    }

    private bool IsInput()
    {
        return _blockInput.IsHolding.CurrentValue;
    }

    private bool CanAttack()
    {
        return !_notInterruptableWindow.IsActive.CurrentValue && !_stamina.IsOnCooldown.CurrentValue;
    }

    private void TryInterrupt(bool _)
    {
        CanInterrupt.Value = CanAttack() && IsInput();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}