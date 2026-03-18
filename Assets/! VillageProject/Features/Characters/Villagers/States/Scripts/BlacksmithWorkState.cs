public class BlacksmithWorkState : WorkVillagerState
{
    private VillagerMovementHandler _movementHandler;
    
    public BlacksmithWorkState(NavmeshMovementAgent navmeshAgent, Profession profession, BuildingStorage buildingStorage)
    {
        var blacksmith = buildingStorage.Get(BuildingType.Blacksmith);
        _movementHandler = new VillagerMovementHandler(navmeshAgent, blacksmith.Data.EnterPoint);
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