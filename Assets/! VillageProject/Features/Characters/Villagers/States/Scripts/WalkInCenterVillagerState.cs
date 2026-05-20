using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class WalkInCenterVillagerState : RelaxVillagerState
{
    private const float MaxRadius = 30;
    private const float StandDuration = 2f;

    private NavmeshMovementAgent _navmeshAgent;
    private VillagerMovementHandler _movementHandler;
    private VillagerData _villagerData;
    
    private Transform _villageCenter;
    private Coroutine _coroutine;
    
    private SkinReferencesResolver _skinReferencesResolver;
    private bool _isFinishing;
    
    public WalkInCenterVillagerState(VillagerData villagerData, NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Transform villageCenter)
    {
        _villagerData = villagerData;
        _skinReferencesResolver = skinReferencesResolver;
        
        _villageCenter = villageCenter;
        
        _navmeshAgent = navmeshAgent;
        _movementHandler = new VillagerMovementHandler(navmeshAgent);
    }
    
    public override void EnterState()
    {
        _isFinishing = false;
        ActivateMovement();
    }
    
    public override void EnterStateWithSkip()
    {
        EnterState();
    }
    
    private void OnMovementEnded(WorkResult result)
    {
        if (_isFinishing || _navmeshAgent == null) return;
        
        if (_coroutine != null) _navmeshAgent.StopCoroutine(_coroutine);
        _coroutine = _navmeshAgent.StartCoroutine(StandRoutine(ActivateMovement));
    }
    
    private void ActivateMovement()
    {
        _movementHandler.ActivateMovementWithPosInCircle(_villageCenter, MaxRadius, callback: OnMovementEnded);
    }

    private IEnumerator StandRoutine(Action callback)
    {
        yield return new WaitForSeconds(StandDuration);
        if (_villagerData.IsTalking) yield return new WaitWhile(() => _villagerData.IsTalking);
        
        callback?.Invoke();
        _coroutine = null;
    }
    
    public override async UniTask ExitState(CancellationToken token)
    {
        _isFinishing = true;
        _movementHandler.DeactivateMovement();
        
        if (_coroutine != null)
        {
            _navmeshAgent.StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
    
    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navmeshAgent.GetCancellationTokenOnDestroy());
    }
    
    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}