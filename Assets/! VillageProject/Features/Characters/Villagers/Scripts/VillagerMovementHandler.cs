using System;
using UnityEngine;

public class VillagerMovementHandler : IDisposable
{
    private NavmeshMovementAgent _navmeshAgent;
    Transform _target;
    
    private IWorkEventSource<WorkResult> _source; 
    private Action<WorkResult> _movementCallback;
    
    public VillagerMovementHandler(NavmeshMovementAgent navmeshAgent, Transform target)
    {
        _navmeshAgent = navmeshAgent;
        _target = target;
    }
    
    public void ActivateMovement(Action<WorkResult> callback = null)
    {
        _navmeshAgent.TrySetInstructions(_target.gameObject, out _source);
        
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