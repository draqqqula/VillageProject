using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public abstract class WorkVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Work;
    
    public override void EnterState()
    {
        
    }
    
    public override async UniTask ExitState(CancellationToken token)
    {

    }

    public override void Dispose()
    {

    }
}