using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class SleepVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Sleep;
    private VillagerData _villagerData;
    private Transform _homePoint;
    
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerMovementHandler _movementHandler;

    public SleepVillagerState(NavmeshMovementAgent navmeshAgent, Transform homePoint, VillagerData villagerData)
    {
        _villagerData = villagerData;
        _homePoint = homePoint;
        
        _navmeshAgent = navmeshAgent;
        _movementHandler = new VillagerMovementHandler(navmeshAgent);
    }
    
    public override void EnterState()
    {
        _movementHandler.ActivateMovement(_homePoint.position, OnMovementEnded);
    }

    private void OnMovementEnded(WorkResult result)
    {
        _villagerData.IsOnHome = true;
        _navmeshAgent.gameObject.SetActive(false);
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _villagerData.IsOnHome = false;
        _navmeshAgent.gameObject.SetActive(true);
        _movementHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}