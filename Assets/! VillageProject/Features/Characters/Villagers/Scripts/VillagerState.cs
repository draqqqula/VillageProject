using System;

public abstract class VillagerState : IDisposable
{
    public abstract ActivityType ActivityType { get; }
    
    public abstract void EnterState();

    public abstract void ExitState();
    public abstract void Dispose();
}