using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

public class BlacksmithWorkState : WorkVillagerState
{
    private Transform target;
    private VillagerData _villagerData;

    private NavmeshMovementAgent _navMeshAgent;
    private VillagerTransformHandler _transformHandler;
    private SkinReferencesResolver _skinReferencesResolver;
    private Profession _profession;
    private BlacksmithProfessionData _blacksmithData;
    
    private BuildingStorage _buildingStorage;
    private RaiseExperienceHandler _experienceHandler;
    private List<RaiseArrowsHandler> _arrowsHandlers = new List<RaiseArrowsHandler>();
    private GameTimer _gameTimer;
    
    private bool _isWorking;
    private bool _isRaisingArrows;
    private SkipTimeController _skipTimeController;
    
    public BlacksmithWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingStorage buildingStorage, GameTimer gameTimer, SkipTimeController skipTimeController,
        VillagerData villagerData)
    {
        _villagerData = villagerData;
        _navMeshAgent = navmeshAgent;
        _profession = profession;
        _blacksmithData = _profession.ProfessionData as BlacksmithProfessionData;
        _skinReferencesResolver = skinReferencesResolver;
        
        _buildingStorage = buildingStorage;
        _buildingStorage.OnBuildingAdded += OnBuildingAdded;
        _skipTimeController = skipTimeController;
        
        var blacksmith = _buildingStorage.Get(BuildingType.Blacksmith);
        target = (blacksmith.Data as WorkBuildingData).WorkPoint;
        
        _gameTimer = gameTimer;
        _transformHandler = new VillagerTransformHandler(navmeshAgent);
        _experienceHandler = new RaiseExperienceHandler(profession, gameTimer);
        _profession.Experience.Subscribe(TryStartRaisingArrows).AddTo(navmeshAgent.gameObject);
        InitRaiseArrowHandlers();
    }
    
    private void InitRaiseArrowHandlers()
    {
        var archerTowers = _buildingStorage.GetAll(BuildingType.ArcherTower);
            
        foreach (var archerTower in archerTowers)
        {
            OnBuildingAdded(archerTower);
        }
    }
    
    private void OnBuildingAdded(Building building)
    {
        if (building.Data.Type == BuildingType.ArcherTower && building.Data is ArcherTowerData archerTowerData)
        {
            var raiseArrowHandler = new RaiseArrowsHandler(_blacksmithData.RaisingAmmunition, archerTowerData.AmmunitionStorage, _gameTimer);
            
            if (_isWorking && _profession.Experience.CurrentValue >= _blacksmithData.ExperienceForRaisingAmmunition)
            {
                var hoursForRaisingAmmunition = (int)Mathf.Ceil(_blacksmithData.RaiseHoursForExperienceCurve.Evaluate(_profession.Experience.CurrentValue));
                raiseArrowHandler.StartRaisingArrows(hoursForRaisingAmmunition);    
            }
            _arrowsHandlers.Add(raiseArrowHandler);
        }
    }
    
    public override void EnterState()
    {
        _transformHandler.ActivateMovementWithRotation(target, callback: OnPointReached);
    }

    public override void EnterStateWithSkip()
    {
        _navMeshAgent.transform.position = target.position;
        
        var distance = Vector3.Distance(_navMeshAgent.transform.position, target.position);
        var moveHours = (int)Mathf.Ceil(distance / _villagerData.SpeedInHour);

        _experienceHandler.IncreaseHours(moveHours);
        foreach (var arrowsHandler in _arrowsHandlers)
        {
            arrowsHandler.IncreaseRaiseArrows(moveHours);
        }
        
        OnPointReached();
    }

    private void OnPointReached()
    {
        _isWorking = true;
        _skinReferencesResolver.Animator.SetBool("Work", true);
        _experienceHandler.StartRaisingExperience();

        TryStartRaisingArrows(_profession.Experience.CurrentValue);
    }
    
    private void TryStartRaisingArrows(float experience)
    {
        if (!_isWorking || _isRaisingArrows) return;
        
        if (experience >= _blacksmithData.ExperienceForRaisingAmmunition)
        {
            _isRaisingArrows = true;
            var hoursForRaisingAmmunition = (int)Mathf.Ceil(_blacksmithData.RaiseHoursForExperienceCurve.Evaluate(experience));
            foreach (var raiseArrowHandler in _arrowsHandlers)
            {
                raiseArrowHandler.StartRaisingArrows(hoursForRaisingAmmunition);
            }
        }
    }
    
    public override async UniTask ExitState(CancellationToken token)
    {
        _transformHandler.DeactivateMovement();

        if (_isWorking)
        {
            _isWorking = false;
            _experienceHandler.StopRaisingExperience();

            if (_isRaisingArrows)
            {
                foreach (var arrowHandler in _arrowsHandlers)
                {
                    arrowHandler.StopRaisingArrows();
                }
                _isRaisingArrows = false;
            }
            
            if (!_skipTimeController.IsSkipping.CurrentValue) await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
            else _skinReferencesResolver.AnimatorHandler.SetBool("Work", false);
        }
    }

    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navMeshAgent.GetCancellationTokenOnDestroy());
    }
    
    public override void Dispose()
    {
        _transformHandler.Dispose();
        _buildingStorage.OnBuildingAdded -= OnBuildingAdded;
    }
}