public class ArmorerWorkState : WorkVillagerState
{
    private VillagerMovementHandler _movementHandler;
    
    public ArmorerWorkState(NavmeshMovementAgent navmeshAgent, Profession profession, BuildingStorage buildingStorage)
    {
        var hospital = buildingStorage.Get(BuildingType.Hospital);
        _movementHandler = new VillagerMovementHandler(navmeshAgent, hospital.Data.EnterPoint);
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