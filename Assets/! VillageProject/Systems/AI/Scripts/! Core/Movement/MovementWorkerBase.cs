using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class MovementWorkerBase : WorkerBase
{
    public abstract float GetVelocityPerSecond();
    public abstract float GetProgress();
    public abstract void HandleCancellation();
}

public abstract class MovementWorkerBase<T> : MovementWorkerBase, IMovementInsructionsAccept<T>
{
    public WorkEventSource CurrentWork;

    protected abstract bool TryAcceptInstructions(T instructions);

    public bool TrySetInstructions(T instructions, out IWorkEventSource<WorkResult> source)
    {
        if (TryAcceptInstructions(instructions))
        {
            CurrentWork = new WorkEventSource();
            SignalWorkAssigned();
            source = CurrentWork;
            return true;
        }
        source = null;
        return false;
    }

    public override void HandleCancellation()
    {
        base.SignalWorkCompleted();
        CurrentWork.Finish(WorkResult.Interrupted);
        CurrentWork = null;
    }

    public void HandleWorkCompleted()
    {
        CurrentWork.Finish(WorkResult.Success);
        CurrentWork = null;
    }

    protected override void SignalWorkCompleted()
    {
        base.SignalWorkCompleted();
        HandleWorkCompleted();
    }
}

public interface IMovementInsructionsAccept<T>
{
    public bool TrySetInstructions(T instructions, out IWorkEventSource<WorkResult> source);
}