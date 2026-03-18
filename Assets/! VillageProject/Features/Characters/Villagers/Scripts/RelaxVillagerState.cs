using UnityEngine;

public sealed class RelaxVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Relax;
    
    private VillagerMovementHandler _movementHandler;

    public RelaxVillagerState(NavmeshMovementAgent navmeshAgent, Transform relaxPoint)
    {
        _movementHandler = new VillagerMovementHandler(navmeshAgent, relaxPoint);
    }
    
    public override void EnterState()
    {
        _movementHandler.ActivateMovement(OnMovementEnded);
    }

    private void OnMovementEnded(WorkResult result)
    {
        
    }

    public override void ExitState()
    {
        _movementHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}