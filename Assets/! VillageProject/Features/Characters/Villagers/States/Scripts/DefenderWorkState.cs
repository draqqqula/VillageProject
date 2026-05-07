using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
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
    
    private SkinReferencesResolver _skinReferencesResolver;
    private bool _isFinishing;
    
    public DefenderWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver, Transform villageCenter)
    {
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

    public override void EnterStateWithSkip() { }

    private void OnMovementEnded(WorkResult result)
    {
        if (_isFinishing) return;
        
        if (_coroutine != null) _navmeshAgent.StopCoroutine(_coroutine);
        _coroutine = _navmeshAgent.StartCoroutine(StandRoutine(ActivateMovement));
    }

    private IEnumerator StandRoutine(Action callback)
    {
        _skinReferencesResolver.Animator.SetBool("Work", true);
        yield return new WaitForSeconds(StandDuration);
        _skinReferencesResolver.Animator.SetBool("Work", false);
        
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
            await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
        }
    }

    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navmeshAgent.GetCancellationTokenOnDestroy());
    }
    
    private void ActivateMovement()
    {
        _movementHandler.ActivateMovementWithPosInCircle(_villageCenter, MaxRadius, MinRadius, OnMovementEnded);
    }
    
    public override void Dispose()
    {
       _movementHandler.Dispose();
       base.Dispose();
    }
}