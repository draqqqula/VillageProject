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
        MoveToBuildingPlace();
    }

    private void MoveToBuildingPlace()
    {
        var plan = _buildingPlanner.GetCurrentPlan();
        
        _movementHandler.SetTargetPos(plan.PreviewObject.Data.EnterPoint.position);
        _movementHandler.ActivateMovement(OnMovementEnded);
    }

    private void OnMovementEnded(WorkResult result)
    {
        if (result == WorkResult.Success)
        {
            _plan = _buildingPlanner.GetCurrentPlan();
            _builtTicks = (int)Mathf.Floor(_gameTimer.ConvertHoursToTick(_plan.HoursDuration) * _plan.BuildingProgress);
            _gameTimer.OnTick += OnTick;
        }
    }

    private void OnTick(int currentTick)
    {
        int deltaTicks = _lastTick >= 0 ? currentTick - _lastTick : 0;
        _lastTick = currentTick;

        _builtTicks += deltaTicks;

        float totalTicks = _gameTimer.ConvertHoursToTick(_plan.HoursDuration);
        float progress = _builtTicks / totalTicks;
        _plan.BuildingProgress = progress;

        if (progress >= 1f)
        {
            _plan.BuildingProgress = 1f;
            FinishBuilding();
        }
    }

    private void FinishBuilding()
    {
        _buildingPlanner.TryCompleteCurrentPlan();
        _gameTimer.OnTick -= OnTick;
        MoveToBuildingPlace();
    }
    
    public override void ExitState()
    {
        if (_plan != null)
        {
            _gameTimer.OnTick -= OnTick;
            _plan = null;
            _builtTicks = -1;
            _lastTick = -1;
        }
        
        _movementHandler.DeactivateMovement();
    }

    public override void Dispose()
    {
       _movementHandler.DeactivateMovement();
    }
}