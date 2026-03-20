using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class DefenderWorkState : WorkVillagerState
{
    private const float MaxRadius = 50;
    private const float MinRadius = 40;
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
        if (_coroutine != null) _navmeshAgent.StopCoroutine(_coroutine);
        _coroutine = _navmeshAgent.StartCoroutine(StandRoutine(ActivateMovement));
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
        _movementHandler.ActivateMovementWithPosInCircle(_villageCenter, MaxRadius, MinRadius, OnMovementEnded);
    }
    
    public override void Dispose()
    {
       _movementHandler.Dispose();
    }
}