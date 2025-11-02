using R3;
using Zenject;

public class GuardToIdleTransition : TransitionBase<GuardState>
{
    [Inject] private IBlockInput _blockInput;

    public override void Construct(GuardState currentState)
    {
        _blockInput.IsHolding.Subscribe(_ => HandleReleased()).AddTo(this);
    }

    private void HandleReleased()
    {
        if (!_blockInput.IsHolding.CurrentValue)
        {
            Activate<IdleState>();
        }
    }
}