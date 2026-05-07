using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public abstract class VillagerState : IDisposable
{
    public abstract ActivityType ActivityType { get; }
    
    public abstract void EnterState();
    public abstract void EnterStateWithSkip();
    
    public abstract UniTask ExitState(CancellationToken token);
    public abstract void ExitStateWithSkip();

    public abstract void Dispose();
}