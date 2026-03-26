using System.Collections;
using UnityEngine;

public class BuilderWorkState : WorkVillagerState
{
    private BuildingPlanner _buildingPlanner;
    private BuildingStorage _buildingStorage;
    
    private VillagerMovementHandler _movementHandler;
    private GameTimer _gameTimer;
    
    private int _builtTicks;
    private int _lastTick = -1;
    private BuildingPlan _plan;
    
    private bool _isActive = false;
    private bool _isBuilding = false;
    
    public BuilderWorkState(NavmeshMovementAgent navmeshAgent, Profession profession, BuildingStorage buildingStorage,
        BuildingPlanner buildingPlanner, GameTimer gameTimer)
    {
        _buildingPlanner = buildingPlanner;
        _buildingStorage = buildingStorage;
        
        _movementHandler = new VillagerMovementHandler(navmeshAgent, navmeshAgent.transform.position);
        _gameTimer = gameTimer;
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
        
        Vector3 enterPoint;
        if (plan is NewBuildingPlan newBuildingPlan) enterPoint = newBuildingPlan.PreviewObject.Data.EnterPoint.position;
        else enterPoint = (plan as RepairingPlan).BrokenBuilding.Data.EnterPoint.position;
        
        _movementHandler.SetTargetPos(enterPoint);
        _movementHandler.ActivateMovement(OnMovementEnded);
    }

    private void OnPlanChanged(BuildingPlan plan)
    {
        if (_plan == plan) return;
        
        _movementHandler.DeactivateMovement();
        if (_isBuilding) FinishBuilding();
        
        MoveToBuildingPlace();
    }

    private void OnMovementEnded(WorkResult result)
    {
        if (result == WorkResult.Success)
        {
            _plan = _buildingPlanner.GetCurrentPlan();
            _lastTick = -1;
            _builtTicks = (int)Mathf.Floor(_gameTimer.ConvertHoursToTick(_plan.HoursDuration) * _plan.BuildingProgress.Value);
            _gameTimer.OnTick += OnTick;
            
            _isBuilding = true;
        }
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
            FinishBuilding();
            MoveToBuildingPlace();
        }
    }

    private void FinishBuilding()
    {
        _buildingPlanner.TryCompleteCurrentPlan();
        _gameTimer.OnTick -= OnTick;
        _plan = null;
        _isBuilding = false;
    }
    
    public override void ExitState()
    {
        if (_isBuilding)
        {
            _gameTimer.OnTick -= OnTick;
            _plan = null;
            _isBuilding = false;
        }
        
        if (_isActive) _buildingPlanner.OnCurrentPlanChanged -= OnPlanChanged;
        _movementHandler.DeactivateMovement();
        
        _isActive = false;
    }

    public override void Dispose()
    {
        if (_isActive) _buildingPlanner.OnCurrentPlanChanged -= OnPlanChanged;
       _movementHandler.DeactivateMovement();
    }
}