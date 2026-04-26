using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class BuilderWorkState : WorkVillagerState
{
    private BuildingPlanner _buildingPlanner;
    private BuildingStorage _buildingStorage;
    
    private NavmeshMovementAgent _navMeshAgent;
    private VillagerTransformHandler _movementHandler;
    private GameTimer _gameTimer;
    
    private int _builtTicks;
    private int _lastTick = -1;
    private BuildingPlan _plan;
    
    private SkinReferencesResolver _skinReferencesResolver;
    private ExperienceHandler _experienceHandler;
    
    private bool _isActive = false;
    private bool _isBuilding = false;
    
    public BuilderWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingStorage buildingStorage, BuildingPlanner buildingPlanner, GameTimer gameTimer)
    {
        _skinReferencesResolver = skinReferencesResolver;
        
        _buildingPlanner = buildingPlanner;
        _buildingStorage = buildingStorage;
        
        _navMeshAgent = navmeshAgent;
        _movementHandler = new VillagerTransformHandler(navmeshAgent);
        _gameTimer = gameTimer;

        _experienceHandler = new ExperienceHandler(profession, _gameTimer);
    }
    
    public override void EnterState()
    {
        _isActive = true;
        
        _buildingPlanner.OnCurrentPlanChanged += OnPlanChanged;
        MoveToBuildingPlace();
    }

    private void MoveToBuildingPlace()
    {
        var plan = _buildingPlanner.GetCurrentPlan();
        
        Transform enterPoint;
        if (plan is NewBuildingPlan newBuildingPlan) enterPoint = newBuildingPlan.PreviewObject.Data.EnterPoint;
        else enterPoint = (plan as RepairingPlan).BrokenBuilding.Data.EnterPoint;
        
        _movementHandler.ActivateMovementWithRotation(enterPoint, callback: OnReachedPoint);
    }

    private void OnPlanChanged(BuildingPlan plan)
    {
        if (_plan == plan) return;
        
        _movementHandler.DeactivateMovement();
        if (_isBuilding && !_skinReferencesResolver.AnimatorHandler.IsTransitioning)
        {
            _ = FinishBuilding(_navMeshAgent.GetCancellationTokenOnDestroy(), MoveToBuildingPlace);
        }
        else if (!_isBuilding) MoveToBuildingPlace();
    }

    private void OnReachedPoint()
    {
        _plan = _buildingPlanner.GetCurrentPlan();
        _lastTick = -1;
        _builtTicks = (int)Mathf.Floor(_gameTimer.ConvertHoursToTick(_plan.HoursDuration) * _plan.BuildingProgress.Value);
        _gameTimer.OnTick += OnTick;
        
        _skinReferencesResolver.Animator.SetBool("Work", true);
        _experienceHandler.StartRaisingExperience();
        _isBuilding = true;
    }

    private void OnTick(int currentTick)
    {
        int deltaTicks = _lastTick >= 0 ? currentTick - _lastTick : 0;
        _lastTick = currentTick;
        _builtTicks += deltaTicks;

        float totalTicks = _gameTimer.ConvertHoursToTick(_plan.HoursDuration);
        float progress = _builtTicks / totalTicks;
        _plan.BuildingProgress.Value = progress;

        if (progress >= 1f)
        {
            _plan.BuildingProgress.Value = 1f;

            if (!_skinReferencesResolver.AnimatorHandler.IsTransitioning)
            {
                _ = FinishBuilding(_navMeshAgent.GetCancellationTokenOnDestroy(), MoveToBuildingPlace);
            }
        }
    }

    private async UniTask FinishBuilding(CancellationToken token, Action callback = null)
    {
        _buildingPlanner.TryCompleteCurrentPlan();
        _gameTimer.OnTick -= OnTick;
        _plan = null;
        
        _experienceHandler.StopRaisingExperience();
        await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
        _isBuilding = false;
        callback?.Invoke();
    }
    
    public override async UniTask ExitState(CancellationToken token)
    {
        if (_isActive) _buildingPlanner.OnCurrentPlanChanged -= OnPlanChanged;
        _movementHandler.DeactivateMovement();
        _isActive = false;
        
        if (_isBuilding)
        {
            _gameTimer.OnTick -= OnTick;
            _plan = null;
            _isBuilding = false;
            _experienceHandler.StopRaisingExperience();
            await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
        }
    }

    public override void Dispose()
    {
        if (_isActive) _buildingPlanner.OnCurrentPlanChanged -= OnPlanChanged;
       _movementHandler.DeactivateMovement();
    }
}