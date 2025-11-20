using R3;

public class InterruptToGuardTransitionChecker : CompositeDisposableBase
{
    private IAnimationWindowListener _notInterruptableWindow;
    private IBlockInput _blockInput;
    private Stamina _stamina;
    
    public ReactiveProperty<bool> CanInterrupt {get; private set;}

    public InterruptToGuardTransitionChecker(IAnimationWindowListener notInterruptableWindow, IBlockInput blockInput,
        Stamina stamina)
    {
        _notInterruptableWindow = notInterruptableWindow;
        _blockInput = blockInput;
        _stamina = stamina;
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

    private void TryInterrupt(bool value)
    {
        CanInterrupt.Value = CanAttack() && IsInput();
    }
}