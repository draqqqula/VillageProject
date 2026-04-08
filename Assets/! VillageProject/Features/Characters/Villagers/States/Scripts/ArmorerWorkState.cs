using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ArmorerWorkState : WorkVillagerState
{
    private Transform target;
    
    private VillagerTransformHandler _movementHandler;
    private SkinReferencesResolver _skinReferencesResolver;

    private bool _isWorking;
    
    public ArmorerWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingStorage buildingStorage)
    {
        _skinReferencesResolver = skinReferencesResolver;
        var hospital = buildingStorage.Get(BuildingType.Hospital);
        target = (hospital.Data as WorkBuildingData).WorkPoint;
        
        _movementHandler = new VillagerTransformHandler(navmeshAgent);
    }
    
    public override void EnterState()
    {
        _movementHandler.ActivateMovementWithRotation(target, callback: OnPointReached);
    }

    private void OnPointReached()
    {
        _isWorking = true;
        _skinReferencesResolver.Animator.SetBool("Work", true);
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _movementHandler.DeactivateMovement();
        
        if (_isWorking)
        {
            _isWorking = false;
            await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
        }
    }

    public override void Dispose()
    {
        _movementHandler.Dispose();
    }
}