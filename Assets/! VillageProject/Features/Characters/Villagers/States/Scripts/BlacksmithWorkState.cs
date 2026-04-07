using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlacksmithWorkState : WorkVillagerState
{
    private Transform target;
    
    private VillagerTransformHandler _transformHandler;
    private SkinReferencesResolver _skinReferencesResolver;
    
    public BlacksmithWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingStorage buildingStorage)
    {
        _skinReferencesResolver = skinReferencesResolver;
        var blacksmith = buildingStorage.Get(BuildingType.Blacksmith);
        target = (blacksmith.Data as WorkBuildingData).WorkPoint;
        
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
    }
    
    public override void EnterState()
    {
        _transformHandler.ActivateMovementWithRotation(target, callback: OnPointReached);
    }

    private void OnPointReached()
    {
        _skinReferencesResolver.Animator.SetBool("Work", true);
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _transformHandler.DeactivateMovement();
        await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
    }
    
    public override void Dispose()
    {
        _transformHandler.Dispose();
    }
}