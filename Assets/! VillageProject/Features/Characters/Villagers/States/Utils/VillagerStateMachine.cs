using System;
using System.Collections.Generic;
using UnityEngine.AI;

public class VillagerStateMachine : IDisposable
{
    public VillagerState CurrentState { get; private set; }
    private NavmeshMovementAgent _navmeshAgent;

    private Dictionary<ActivityType, VillagerState> _states = new Dictionary<ActivityType, VillagerState>();
    
    private VillagerStateFactory _stateFactory;
    private BuildingStorage _buildingStorage;

    public VillagerStateMachine(VillagerData villagerData, NavmeshMovementAgent navmeshAgent, BuildingStorage buildingStorage)
    {
        _navmeshAgent = navmeshAgent;
        _stateFactory = new VillagerStateFactory();
        _buildingStorage = buildingStorage;
        
        SetStates(villagerData);
    }

    public void SetStates(VillagerData villagerData)
    {
        _stateFactory.SetParams(_navmeshAgent, villagerData, _buildingStorage);
        
        _states.Add(ActivityType.Sleep, _stateFactory.CreateSleepState());
        _states.Add(ActivityType.Work, _stateFactory.CreateWorkState());
        _states.Add(ActivityType.Relax, _stateFactory.CreateRelaxState());
        _states.Add(ActivityType.Guard, _stateFactory.CreateGuardState());
    }

    public void UpdateCurrentState(ActivityType activityType)
    {
        CurrentState?.ExitState();
        CurrentState = _states[activityType];
        CurrentState.EnterState();
    }

    public void Dispose()
    {
        foreach (var state in _states.Values)
        {
            state.Dispose();
        }
    }
}