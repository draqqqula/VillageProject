using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class SleepVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Sleep;
    private VillagerData _villagerData;
    private SkinReferencesResolver _skinReferencesResolver;
    
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerTransformHandler _transformHandler;
    private VillagerFadingHandler _fadingHandler;
    
    private SkipTimeController _skipTimeController;

    public SleepVillagerState(NavmeshMovementAgent navmeshAgent, VillagerData villagerData, SkinReferencesResolver skinReferencesResolver,
        SkipTimeController skipTimeController)
    {
        _villagerData = villagerData;
        _skinReferencesResolver = skinReferencesResolver;
        
        _navmeshAgent = navmeshAgent;
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        _fadingHandler = new VillagerFadingHandler(skinReferencesResolver);
        _skipTimeController = skipTimeController;
    }
    
    public override void EnterState()
    {
        _transformHandler.ActivateMovementWithRotation(_villagerData.HomePoint.DoorPoint, callback: OnPointReached);
    }

    public override void EnterStateWithSkip()
    {
        _fadingHandler.FadeOutImmediately();
        OnFadingEnded();
    }
    
    private void OnPointReached()
    {
        _fadingHandler.FadeOut(OnFadingEnded);
    }

    private void OnFadingEnded()
    {
        _villagerData.IsOnHome = true;
        _navmeshAgent.UnconnectFromNavmeshManually();
        _navmeshAgent.transform.position = _villagerData.HomePoint.Point.position;
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        if (_fadingHandler.IsFading) await _fadingHandler.WaitFading(token);
        
        if (_villagerData.IsOnHome)
        {
            _navmeshAgent.UnconnectFromNavmeshManually();
            _navmeshAgent.transform.position = _villagerData.HomePoint.DoorPoint.position;
            _navmeshAgent.ConnectToNavmeshManually();
            
            if (!_skipTimeController.IsSkipping.CurrentValue) await _fadingHandler.FadeInAsync(token);
            else _fadingHandler.FadeInImmediately();
            
            _villagerData.IsOnHome = false;
        }

        _transformHandler.DeactivateMovement();
    }

    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navmeshAgent.GetCancellationTokenOnDestroy());
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
        _fadingHandler.Dispose();
    }
}