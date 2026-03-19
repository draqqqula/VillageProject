using System;
using UnityEngine;

public class VillagerMovementHandler : IDisposable
{
    private NavmeshMovementAgent _navmeshAgent;
    Vector3 _targetPos;
    
    private IWorkEventSource<WorkResult> _source; 
    private Action<WorkResult> _movementCallback;
    
    public VillagerMovementHandler(NavmeshMovementAgent navmeshAgent, Vector3 targetPos)
    {
        _navmeshAgent = navmeshAgent;
        SetTargetPos(targetPos);
    }

    public void SetTargetPos(Vector3 targetPos)
    {
        _targetPos = targetPos;
    }
    
    public void ActivateMovement(Action<WorkResult> callback = null)
    {
        _navmeshAgent.TrySetInstructions(_targetPos, out _source);
        
        _movementCallback = callback;
        if (_movementCallback != null)
        {
            _source.OnFinished += _movementCallback;
        }
    }

    public void DeactivateMovement()
    {
        if (_movementCallback != null && _source != null)
        {
            _source.OnFinished -= _movementCallback;
        }
        _navmeshAgent.StopAgent();
    }

    public void Dispose()
    {
        if (_movementCallback != null && _source != null)
        {
            _source.OnFinished -= _movementCallback;
        }
    }
}