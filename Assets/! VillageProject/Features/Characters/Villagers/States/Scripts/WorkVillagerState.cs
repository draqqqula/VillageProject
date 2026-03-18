using System;

public abstract class WorkVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Work;
    
    public override void EnterState()
    {
        
    }
    
    public override void ExitState()
    {
        
    }

    public override void Dispose()
    {
       
    }
}

public class DefenderWorkState : WorkVillagerState
{
    public DefenderWorkState(NavmeshMovementAgent navmeshAgent, Profession profession)
    {
    }
}

public class BuilderWorkState : WorkVillagerState
{
    public BuilderWorkState(NavmeshMovementAgent navmeshAgent, Profession profession, BuildingStorage buildingStorage)
    {
    }
}