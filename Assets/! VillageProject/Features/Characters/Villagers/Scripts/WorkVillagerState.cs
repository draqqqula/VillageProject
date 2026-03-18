public class WorkVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Work;
    
    private VillagerMovementHandler _movementHandler;

    public WorkVillagerState(NavmeshMovementAgent navmeshAgent, Profession profession)
    {
        _movementHandler = new VillagerMovementHandler(navmeshAgent, profession.WorkPoint);
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

public class BlacksmithWorkState : WorkVillagerState
{
    public BlacksmithWorkState(NavmeshMovementAgent navmeshAgent, Profession profession) : base(navmeshAgent, profession)
    {
    }
}

public class ArcherWorkState : WorkVillagerState
{
    public ArcherWorkState(NavmeshMovementAgent navmeshAgent, Profession profession) : base(navmeshAgent, profession)
    {
    }
}

public class DefenderWorkState : WorkVillagerState
{
    public DefenderWorkState(NavmeshMovementAgent navmeshAgent, Profession profession) : base(navmeshAgent, profession)
    {
    }
}

public class BuilderWorkState : WorkVillagerState
{
    public BuilderWorkState(NavmeshMovementAgent navmeshAgent, Profession profession) : base(navmeshAgent, profession)
    {
    }
}

public class ArmorerWorkState : WorkVillagerState
{
    public ArmorerWorkState(NavmeshMovementAgent navmeshAgent, Profession profession) : base(navmeshAgent, profession)
    {
    }
}