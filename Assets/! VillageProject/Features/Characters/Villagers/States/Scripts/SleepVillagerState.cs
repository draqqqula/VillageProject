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

    public SleepVillagerState(NavmeshMovementAgent navmeshAgent, VillagerData villagerData, SkinReferencesResolver skinReferencesResolver)
    {
        _villagerData = villagerData;
        _skinReferencesResolver = skinReferencesResolver;
        
        _navmeshAgent = navmeshAgent;
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        _fadingHandler = new VillagerFadingHandler(skinReferencesResolver);
    }
    
    public override void EnterState()
    {
        _transformHandler.ActivateMovementWithRotation(_villagerData.HomePoint.DoorPoint, callback: OnPointReached);
    }

    private void OnPointReached()
    {
        _fadingHandler.FadeOut(OnFadingEnded);
    }

    private void OnFadingEnded()
    {
        _villagerData.IsOnHome = true;
        _navmeshAgent.enabled = false;
        _navmeshAgent.transform.position = _villagerData.HomePoint.Point.position;
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        if (_fadingHandler.IsFading) await _fadingHandler.WaitFading(token);
        
        if (_villagerData.IsOnHome)
        {
            _navmeshAgent.transform.position = _villagerData.HomePoint.DoorPoint.position;
            _navmeshAgent.enabled = true;
            await _fadingHandler.FadeInAsync(token);
            _villagerData.IsOnHome = false;
        }

        _transformHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
        _fadingHandler.Dispose();
    }
}