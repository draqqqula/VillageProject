using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlacksmithWorkState : WorkVillagerState
{
    private Transform target;
    
    private VillagerTransformHandler _transformHandler;
    private SkinReferencesResolver _skinReferencesResolver;
    
    private ExperienceHandler _experienceHandler;
    
    private bool _isWorking;
    
    public BlacksmithWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingStorage buildingStorage, GameTimer gameTimer)
    {
        _skinReferencesResolver = skinReferencesResolver;
        var blacksmith = buildingStorage.Get(BuildingType.Blacksmith);
        target = (blacksmith.Data as WorkBuildingData).WorkPoint;
        
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        _experienceHandler = new ExperienceHandler(profession, gameTimer);
    }
    
    public override void EnterState()
    {
        _transformHandler.ActivateMovementWithRotation(target, callback: OnPointReached);
    }

    private void OnPointReached()
    {
        _isWorking = true;
        _skinReferencesResolver.Animator.SetBool("Work", true);
        _experienceHandler.StartRaisingExperience();
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _transformHandler.DeactivateMovement();

        if (_isWorking)
        {
            _isWorking = false;
            _experienceHandler.StopRaisingExperience();
            await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
        }
    }
    
    public override void Dispose()
    {
        _transformHandler.Dispose();
    }
}