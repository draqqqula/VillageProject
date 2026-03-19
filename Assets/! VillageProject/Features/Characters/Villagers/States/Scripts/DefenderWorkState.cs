using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class DefenderWorkState : WorkVillagerState
{
    private const float MaxRadius = 50;
    private const float MinRadius = 40;
    private const int MaxPointAttempts = 10;
    private const float StandDuration = 2f;

    private NavmeshMovementAgent _navmeshAgent;
    private VillagerMovementHandler _movementHandler;
    
    private Transform _villageCenter;
    private Coroutine _coroutine;
    
    public DefenderWorkState(NavmeshMovementAgent navmeshAgent, Profession profession, Transform villageCenter)
    {
        _villageCenter = villageCenter;
        
        _navmeshAgent = navmeshAgent;
        _movementHandler = new VillagerMovementHandler(navmeshAgent, _villageCenter.position);
    }
    
    public override void EnterState()
    {
        ActivateMovement();
    }
    
    private void OnMovementEnded(WorkResult result)
    {
        if (_coroutine != null)
        {
            _navmeshAgent.StopCoroutine(_coroutine);
            _coroutine = null;
        }
        else _coroutine = _navmeshAgent.StartCoroutine(StandRoutine(ActivateMovement));
    }

    private IEnumerator StandRoutine(Action callback)
    {
        yield return new WaitForSeconds(StandDuration);
        callback?.Invoke();
        _coroutine = null;
    }

    public override void ExitState()
    {
        _movementHandler.DeactivateMovement();
        
        if (_coroutine != null)
        {
            _navmeshAgent.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
    
    private void ActivateMovement()
    {
        var pos = GetMovePos();
        var go = new GameObject("DefenderTarget");
        go.transform.position = pos;
        
        _movementHandler.SetTargetPos(pos);
        _movementHandler.ActivateMovement(OnMovementEnded);
    }
    
    private Vector3 GetMovePos()
    {
        for (int i = 0; i < MaxPointAttempts; i++)
        {
            var dir2D = Random.insideUnitCircle.normalized;
            var dir = new Vector3(dir2D.x, 0, dir2D.y);
            
            var distance = Random.Range(MinRadius, MaxRadius);
            var pos = _villageCenter.position + dir * distance;
            
            if (IsOnStreet(pos)) return pos;
        }
        return _navmeshAgent.transform.position;
    }
    
    private bool IsOnStreet(Vector3 pos)
    {
        float rayDistance = 10;
        var rayOrigin = new Vector3(pos.x, _villageCenter.position.y + rayDistance / 2, pos.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Low"))
            {
                return true;
            }
        }
        return false;
    }
    
    public override void Dispose()
    {
       _movementHandler.Dispose();
    }
}