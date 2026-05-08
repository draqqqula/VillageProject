using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public sealed class PeacefulVillagerGuardState : GuardVillagerState
{
    private const float MaxRadius = 50; 
    
    private VillagerData _villagerData;
    private SkinReferencesResolver _skinReferencesResolver;
    
    private Transform _villageCenter;
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerTransformHandler _transformHandler;
    private VillagerFadingHandler _fadingHandler;
    
    private Coroutine _coroutine;

    public PeacefulVillagerGuardState(NavmeshMovementAgent navmeshAgent, Transform villageCenter, VillagerData villagerData, 
        SkinReferencesResolver skinReferencesResolver)
    {
        _villagerData = villagerData;
        _skinReferencesResolver = skinReferencesResolver;
        
        _villageCenter = villageCenter;
        _navmeshAgent = navmeshAgent;
        
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        _fadingHandler = new VillagerFadingHandler(skinReferencesResolver);
        _villagerData.HomePoint.IsAttacked.Skip(1).Subscribe(OnHomeAttacked).AddTo(_navmeshAgent.gameObject);
    }
    
    public override void EnterState()
    {
        _skinReferencesResolver.Animator.SetBool("Scared", true);
        
        if (!_villagerData.HomePoint.IsAttacked.Value) MoveToHome();
        else MoveToRandomPoint();
    }
    
    private void MoveToHome()
    {
        _transformHandler.ActivateMovementWithRotation(_villagerData.HomePoint.DoorPoint, callback: OnHomeReached);
    }

    private void MoveToRandomPoint()
    {
        _transformHandler.ActivateMovementWithPosInCircle(_villageCenter, MaxRadius, callback: OnPointReached);
    }
    
    private void OnHomeAttacked(bool value)
    {
        if (value)
        {
            MoveToRandomPoint();
        }
    }

    private void OnHomeReached()
    {
        _fadingHandler.FadeOut(OnFadingEnded);
    }

    private void OnFadingEnded()
    {
        _villagerData.IsOnHome = true;
        _navmeshAgent.UnconnectFromNavmeshManually();
        _navmeshAgent.transform.position = _villagerData.HomePoint.Point.position;
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

    public override async UniTask ExitState(CancellationToken token)
    {
        if (_fadingHandler.IsFading) await _fadingHandler.WaitFading(token);
        
        _transformHandler.DeactivateMovement();
        if (_coroutine != null)
        {
            _navmeshAgent.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (_villagerData.IsOnHome)
        {
            _navmeshAgent.UnconnectFromNavmeshManually();
            _navmeshAgent.transform.position = _villagerData.HomePoint.DoorPoint.position;
            _navmeshAgent.ConnectToNavmeshManually();
            await _fadingHandler.FadeInAsync(token);
            
            _villagerData.IsOnHome = false;
        }
        await _skinReferencesResolver.AnimatorHandler.TransitByBool("Scared", false, _navmeshAgent.GetCancellationTokenOnDestroy());
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
    }
}