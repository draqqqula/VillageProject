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