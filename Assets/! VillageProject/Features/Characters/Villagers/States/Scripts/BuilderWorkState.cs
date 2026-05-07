using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using R3;

public class BuilderWorkState : WorkVillagerState
{
    private const int FirstMoveHours = 2;
    private const int SecondaryMoveHours = 1;
    private const int MinDistanceToBuilding = 2;
    
    private Profession _profession;
    private BuildingPlanner _buildingPlanner;
    
    private NavmeshMovementAgent _navMeshAgent;
    private VillagerTransformHandler _movementHandler;
    private BuildingPlan _plan;
    
    private SkinReferencesResolver _skinReferencesResolver;
    private RaiseExperienceHandler _experienceHandler;
    private BuildingProgressHandler _buildingProgressHandler;
    
    private GameTimer _gameTimer;
    
    private bool _isActive = false;
    private bool _isBuilding = false;
    private bool _isFinishing = false;
    private SkipTimeController _skipTimeController;
    
    public BuilderWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingPlanner buildingPlanner, GameTimer gameTimer, SkipTimeController skipTimeController)
    {
        _profession = profession;
        _skinReferencesResolver = skinReferencesResolver;
        
        _buildingPlanner = buildingPlanner;
        
        _navMeshAgent = navmeshAgent;
        _movementHandler = new VillagerTransformHandler(navmeshAgent);

        _experienceHandler = new RaiseExperienceHandler(profession, gameTimer);
        _buildingProgressHandler = new BuildingProgressHandler(gameTimer);
        _skipTimeController = skipTimeController;

        _profession.Experience.Subscribe(TryDecreasePlanDuration).AddTo(navmeshAgent.gameObject);
        _gameTimer = gameTimer;
    }
    
    public override void EnterState()
    {
        _isActive = true;
        
        _buildingPlanner.OnCurrentPlanChanged += OnPlanChanged;
        MoveToBuildingPlace();
    }

    public override void EnterStateWithSkip()
    {
        _isActive = true;
        _buildingPlanner.OnCurrentPlanChanged += OnPlanChanged;
        TeleportToBuildingPlace(true);
    }

    private void MoveToBuildingPlace()
    {
        var plan = _buildingPlanner.GetCurrentPlan();
        
        Transform enterPoint;
        if (plan is NewBuildingPlan newBuildingPlan) enterPoint = newBuildingPlan.PreviewObject.Data.EnterPoint;
        else enterPoint = (plan as RepairingPlan).BrokenBuilding.Data.EnterPoint;
        
        _movementHandler.ActivateMovementWithRotation(enterPoint, callback: OnReachedPoint);
    }

    private void TeleportToBuildingPlace() => TeleportToBuildingPlace(false);
    
    private void TeleportToBuildingPlace(bool isFirstBuilding)
    {
        var plan = _buildingPlanner.GetCurrentPlan();
        Transform enterPoint;
            
        if (plan is NewBuildingPlan newBuildingPlan) enterPoint = newBuildingPlan.PreviewObject.Data.EnterPoint;
        else enterPoint = (plan as RepairingPlan).BrokenBuilding.Data.EnterPoint;
        
        if (Vector3.Distance(_navMeshAgent.transform.position, enterPoint.position) > MinDistanceToBuilding)
        {
            var hours = isFirstBuilding ? FirstMoveHours : SecondaryMoveHours;
            _buildingProgressHandler.IncreaseBuildDuration(hours);
            _experienceHandler.IncreaseHours(hours);
        }
        
        _navMeshAgent.enabled = false;
        _navMeshAgent.transform.position = enterPoint.position;
        _navMeshAgent.enabled = true;
        OnReachedPoint();
    }

    private void OnPlanChanged(BuildingPlan plan)
    {
        if (_isFinishing || _plan == plan) return;
        
        _movementHandler.DeactivateMovement();

        Action callback = null;
        if (!_skipTimeController.IsSkipping.CurrentValue) callback = MoveToBuildingPlace;
        else callback = TeleportToBuildingPlace;
        
        if (_isBuilding)
        {
            _ = FinishBuilding(_navMeshAgent.GetCancellationTokenOnDestroy(), callback);
        }
        else if (!_isBuilding) callback?.Invoke();
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
        Debug.Log("OnPlanCompleted");

        if (!_isFinishing && !_skipTimeController.IsSkipping.CurrentValue)
        {
            _ = FinishBuilding(_navMeshAgent.GetCancellationTokenOnDestroy(), MoveToBuildingPlace);
        }
        else if (_skipTimeController.IsSkipping.CurrentValue)
        {
            _ = FinishBuilding(_navMeshAgent.GetCancellationTokenOnDestroy(), TeleportToBuildingPlace);
        } 
    }
    
    private async UniTask FinishBuilding(CancellationToken token, Action callback = null)
    {
        _isFinishing = true;
        _buildingPlanner.TryCompleteCurrentPlan();
        _buildingProgressHandler.OnPlanCompleted -= OnPlanCompleted;
        _plan = null;
        
        _buildingProgressHandler.StopRaisingProgress();
        _experienceHandler.StopRaisingExperience();
        
        if (!_skipTimeController.IsSkipping.CurrentValue)
            await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
        else _skinReferencesResolver.Animator.SetBool("Work", false);
        
        _isBuilding = false;
        _isFinishing = false;
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
            _isFinishing = false;
            
            _buildingProgressHandler.StopRaisingProgress();
            _experienceHandler.StopRaisingExperience();
            
            if (!_skipTimeController.IsSkipping.CurrentValue) 
                await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
            else _skinReferencesResolver.AnimatorHandler.SetBool("Work", false);
        }
    }

    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navMeshAgent.GetCancellationTokenOnDestroy());
    }

    public override void Dispose()
    {
        if (_isActive) _buildingPlanner.OnCurrentPlanChanged -= OnPlanChanged;
       _movementHandler.DeactivateMovement();
    }
}