using System;
using System.Collections;
using R3;
using UnityEngine;

public sealed class PeacefulVillagerGuardState : GuardVillagerState
{
    private const float MaxRadius = 50; 
    
    private VillagerData _villagerData;
    
    private Transform _villageCenter;
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerMovementHandler _movementHandler;
    
    private Coroutine _coroutine;

    public PeacefulVillagerGuardState(NavmeshMovementAgent navmeshAgent, Transform villageCenter, VillagerData villagerData)
    {
        _villagerData = villagerData;
        
        _villageCenter = villageCenter;
        _navmeshAgent = navmeshAgent;
        _movementHandler = new VillagerMovementHandler(navmeshAgent, _villagerData.HomePoint.DoorPoint.position);
    }
    
    public override void EnterState()
    {
        _villagerData.HomePoint.IsAttacked.Skip(1).Subscribe(OnHomeAttacked).AddTo(_navmeshAgent.gameObject);
        
        if (!_villagerData.HomePoint.IsAttacked.Value) MoveToHome();
        else MoveToRandomPoint();
    }
    
    private void MoveToHome()
    {
        _movementHandler.SetTargetPos(_villagerData.HomePoint.DoorPoint.position);
        _movementHandler.ActivateMovement(OnHomeReached);
    }

    private void MoveToRandomPoint()
    {
        _movementHandler.ActivateMovementWithPosInCircle(_villageCenter, MaxRadius, callback: OnPointReached);
    }
    
    private void OnHomeAttacked(bool value)
    {
        if (value)
        {
            MoveToRandomPoint();
        }
    }

    private void OnHomeReached(WorkResult workResult)
    {
        if (workResult == WorkResult.Success)
        {
            _villagerData.IsOnHome = true;
            _navmeshAgent.gameObject.SetActive(false);
        }
    }

    private void OnPointReached(WorkResult workResult)
    {
        if (workResult == WorkResult.Success)
        {
            if (_coroutine != null) _navmeshAgent.StopCoroutine(_coroutine);
            _navmeshAgent.StartCoroutine(StandRoutine(MoveToRandomPoint));
        }
    }

    private IEnumerator StandRoutine(Action callback)
    {
        yield return null;
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

    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}