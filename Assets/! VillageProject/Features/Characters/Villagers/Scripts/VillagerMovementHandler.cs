using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VillagerMovementHandler : IDisposable
{
    private const int MaxPointAttempts = 10;
    
    private NavmeshMovementAgent _navmeshAgent;
    Vector3 _targetPos;
    
    private IWorkEventSource<WorkResult> _source; 
    private Action<WorkResult> _movementCallback;
    
    public VillagerMovementHandler(NavmeshMovementAgent navmeshAgent)
    {
        _navmeshAgent = navmeshAgent;
    }
    
    public void ActivateMovement(Vector3 targetPos, Action<WorkResult> callback = null)
    {
        _navmeshAgent.TrySetInstructions(targetPos, out _source);
        
        _movementCallback = callback;
        if (_movementCallback != null)
        {
            _source.OnFinished += _movementCallback;
        }
    }

    public void ActivateMovementWithPosInCircle(Transform transformCenter, float maxRadius, float minRadius = 0, 
        Action<WorkResult> callback = null)
    {
        var pos = GetMovePos(transformCenter, maxRadius, minRadius);
        ActivateMovement(pos, callback);
    }

    public void DeactivateMovement()
    {
        if (_movementCallback != null && _source != null)
        {
            _source.OnFinished -= _movementCallback;
        }
        _navmeshAgent.StopAgent();
    }
    
    private Vector3 GetMovePos(Transform transformCenter, float maxRadius, float minRadius = 0)
    {
        for (int i = 0; i < MaxPointAttempts; i++)
        {
            var dir2D = Random.insideUnitCircle.normalized;
            var dir = new Vector3(dir2D.x, 0, dir2D.y);
            
            var distance = Random.Range(minRadius, maxRadius);
            var pos = transformCenter.position + dir * distance;
            
            if (IsOnStreet(pos, transformCenter)) return pos;
        }
        return _navmeshAgent.transform.position;
    }
    
    private bool IsOnStreet(Vector3 pos, Transform transformCenter)
    {
        float rayDistance = 10;
        var rayOrigin = new Vector3(pos.x, transformCenter.position.y + rayDistance / 2, pos.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Low"))
            {
                return true;
            }
        }
        return false;
    }

    public void Dispose()
    {
        if (_movementCallback != null && _source != null)
        {
            _source.OnFinished -= _movementCallback;
        }
    }
}