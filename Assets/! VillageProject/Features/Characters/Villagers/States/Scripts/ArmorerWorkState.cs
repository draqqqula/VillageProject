using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ArmorerWorkState : WorkVillagerState
{
    private Transform target;
    
    private VillagerTransformHandler _movementHandler;
    private SkinReferencesResolver _skinReferencesResolver;
    
    public ArmorerWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingStorage buildingStorage)
    {
        _skinReferencesResolver = skinReferencesResolver;
        var hospital = buildingStorage.Get(BuildingType.Hospital);
        target = hospital.Data.EnterPoint;
        
        _movementHandler = new VillagerTransformHandler(navmeshAgent);
    }
    
    public override void EnterState()
    {
        _movementHandler.ActivateMovementWithRotation(target, 2, OnPointReached);
    }

    private void OnPointReached()
    {
        _skinReferencesResolver.Animator.SetBool("Work", true);
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _movementHandler.DeactivateMovement();
        await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
    }

    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}