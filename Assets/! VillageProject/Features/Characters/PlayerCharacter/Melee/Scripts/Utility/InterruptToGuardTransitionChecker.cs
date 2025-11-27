using R3;
using System;
using UnityEngine;
using Zenject;

public class InterruptToGuardTransitionChecker : CompositeDisposableBase
{
    [Inject(Id = "NotInterruptable")] private IAnimationWindowListener _notInterruptableWindow;
    [Inject] private IBlockInput _blockInput;
    [Inject] private Stamina _stamina;
    
    public ReactiveProperty<bool> CanInterrupt {get; private set;}

    public void Initialize()
    {
        CanInterrupt = new ReactiveProperty<bool>(false);
        _notInterruptableWindow.IsActive.Subscribe(TryInterrupt).AddTo(this);
        _blockInput.IsHolding.Subscribe(TryInterrupt).AddTo(this);
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
        Debug.Log("TryInterrupt Window:" + (!_notInterruptableWindow.IsActive.CurrentValue).ToString() + " Stamina:"+ (!_stamina.IsOnCooldown.CurrentValue).ToString() + " Input:" + _blockInput.IsHolding.CurrentValue.ToString());
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}