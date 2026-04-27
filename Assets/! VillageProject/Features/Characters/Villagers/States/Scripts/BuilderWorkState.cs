using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using R3;

public class BuilderWorkState : WorkVillagerState
{
    private Profession _profession;
    private BuildingPlanner _buildingPlanner;
    
    private NavmeshMovementAgent _navMeshAgent;
    private VillagerTransformHandler _movementHandler;
    private BuildingPlan _plan;
    
    private SkinReferencesResolver _skinReferencesResolver;
    private RaiseExperienceHandler _experienceHandler;
    private BuildingProgressHandler _buildingProgressHandler;
    
    private bool _isActive = false;
    private bool _isBuilding = false;
    
    public BuilderWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingPlanner buildingPlanner, GameTimer gameTimer)
    {
        _profession = profession;
        _skinReferencesResolver = skinReferencesResolver;
        
        _buildingPlanner = buildingPlanner;
        
        _navMeshAgent = navmeshAgent;
        _movementHandler = new VillagerTransformHandler(navmeshAgent);

        _experienceHandler = new RaiseExperienceHandler(profession, gameTimer);
        _buildingProgressHandler = new BuildingProgressHandler(gameTimer);

        _profession.Experience.Subscribe(TryDecreasePlanDuration).AddTo(navmeshAgent.gameObject);
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
        _buildingProgressHandler.SetPlan(_plan);
        _buildingProgressHandler.OnPlanCompleted += OnPlanCompleted;
        
        _skinReferencesResolver.Animator.SetBool("Work", true);
        TryDecreasePlanDuration(_profession.Experience.CurrentValue);
        
        _buildingProgressHandler.StartRaisingProgress();
        _experienceHandler.StartRaisingExperience();
        
        _isBuilding = true;
    }

    private void TryDecreasePlanDuration(float experience)
    {
        var multiplier = (_profession.ProfessionData as BuilderProfessionData).PlanDurationMultiplierCurve.Evaluate(experience);
        foreach (var plan in _buildingPlanner.PriorityBuildingPlans)
        {
            if (plan == _plan) continue;
            
            var hours = plan.BuildingProgress.CurrentValue * plan.HoursDuration;
            plan.HoursDuration = (int)Mathf.Ceil(plan.DefaultHoursDuration * multiplier);
            plan.BuildingProgress.Value = Mathf.Clamp01(hours / plan.HoursDuration);
        }
    }

    private void OnPlanCompleted()
    {
        _plan.BuildingProgress.Value = 1f;

        if (!_skinReferencesResolver.AnimatorHandler.IsTransitioning)
        {
            _ = FinishBuilding(_navMeshAgent.GetCancellationTokenOnDestroy(), MoveToBuildingPlace);
        }
    }
    
    private async UniTask FinishBuilding(CancellationToken token, Action callback = null)
    {
        _buildingPlanner.TryCompleteCurrentPlan();
        _buildingProgressHandler.OnPlanCompleted -= OnPlanCompleted;
        _plan = null;
        
        _buildingProgressHandler.StopRaisingProgress();
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
            _buildingProgressHandler.OnPlanCompleted -= OnPlanCompleted;
            _plan = null;
            _isBuilding = false;
            
            _buildingProgressHandler.StopRaisingProgress();
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