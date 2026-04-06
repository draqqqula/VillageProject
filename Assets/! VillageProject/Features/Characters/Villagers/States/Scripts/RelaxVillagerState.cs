using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class RelaxVillagerState : VillagerState
{
    public override ActivityType ActivityType => ActivityType.Relax;

    private Transform _target;
    private VillagerTransformHandler _transformHandler;

    public RelaxVillagerState(NavmeshMovementAgent navmeshAgent, Transform relaxPoint)
    {
        _target = relaxPoint;
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
    }
    
    public override void EnterState()
    {
        _transformHandler.ActivateMovementWithRotation(_target, 2, OnPointReached);
    }

    private void OnPointReached()
    {
        
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _transformHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
        _transformHandler.Dispose();
    }
}