using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using BezierSolution;

[Serializable, GeneratePropertyBag]
public abstract partial class StartMovementActionBase<TAgent, TInstructions> : 
    Action where TAgent : MonoBehaviour, IMovementInsructionsAccept<TInstructions>
{
    private IWorkEventSource<WorkResult> _source;
    private bool _finished;

    public abstract TAgent InstructionsAcceptor { get; }
    public abstract TInstructions Instructions { get; }

    protected override Status OnStart()
    {
        if (InstructionsAcceptor.TrySetInstructions(Instructions, out _source))
        {
            BindWork();
            return Status.Running;
        }
        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return _finished ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        UnbindWork();
        // TODO: force Finish with status cancelled when interrupted
    }

    private void HandleFinished(WorkResult result)
    {
        UnbindWork();
    }

    private void BindWork()
    {
        _source.OnFinished += HandleFinished;
        _finished = false;
    }

    private void UnbindWork()
    {
        if (_source != null)
        {
            _source.OnFinished -= HandleFinished;
        }
        _source = null;
        _finished = true;
    }
}

