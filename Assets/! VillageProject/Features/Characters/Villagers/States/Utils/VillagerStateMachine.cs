using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerStateMachine : IDisposable
{
    public VillagerState CurrentState { get; private set; }
    private NavmeshMovementAgent _navmeshAgent;
    private SearchForTarget _searchForTarget;
    private Animator _animator;

    private Dictionary<ActivityType, VillagerState> _states = new Dictionary<ActivityType, VillagerState>();
    
    private VillagerStateFactory _stateFactory;
    private BuildingStorage _buildingStorage;
    private BuildingPlanner _buildingPlanner;
    
    private Transform _villageCenter;
    private GameTimer _gameTimer;

    public VillagerStateMachine(VillagerData villagerData, NavmeshMovementAgent navmeshAgent, SearchForTarget searchForTarget, 
        SkinReferencesResolver skinReferencesResolver, BuildingStorage buildingStorage, BuildingPlanner buildingPlanner, Transform villageCenter,
        GameTimer gameTimer)
    {
        _navmeshAgent = navmeshAgent;
        _searchForTarget = searchForTarget;
        _stateFactory = new VillagerStateFactory();
        
        _buildingStorage = buildingStorage;
        _buildingPlanner = buildingPlanner;
        
        _villageCenter = villageCenter;
        _gameTimer = gameTimer;
        
        SetStates(villagerData, skinReferencesResolver);
    }

    public void SetStates(VillagerData villagerData, SkinReferencesResolver skinReferencesResolver)
    {
        _animator = skinReferencesResolver.Animator;
        _stateFactory.SetParams(_navmeshAgent, villagerData,  _searchForTarget, _animator, _buildingStorage, _buildingPlanner,
            _villageCenter, _gameTimer);
        _states.Clear();
        
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

    public void Update()
    {
        if (CurrentState is IUpdatableState updatableState) updatableState.Update();
    }

    public void Dispose()
    {
        foreach (var state in _states.Values)
        {
            state.Dispose();
        }
    }
}