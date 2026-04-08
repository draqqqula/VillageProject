using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class SleepVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Sleep;
    private VillagerData _villagerData;
    
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerTransformHandler _transformHandler;

    public SleepVillagerState(NavmeshMovementAgent navmeshAgent, VillagerData villagerData)
    {
        _villagerData = villagerData;
        
        _navmeshAgent = navmeshAgent;
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
    }
    
    public override void EnterState()
    {
        _transformHandler.ActivateMovementWithRotation(_villagerData.HomePoint.DoorPoint, callback: OnPointReached);
    }

    private void OnPointReached()
    {
        _villagerData.IsOnHome = true;
        _navmeshAgent.enabled = false;
        _navmeshAgent.transform.position = _villagerData.HomePoint.Point.position;
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        if (_villagerData.IsOnHome)
        {
            _villagerData.IsOnHome = false;
            _navmeshAgent.transform.position = _villagerData.HomePoint.DoorPoint.position;
            _navmeshAgent.enabled = true;
        }

        _transformHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
    }
}