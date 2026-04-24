using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class VillagerStateMachine : IDisposable
{
    public VillagerState CurrentState { get; private set; }
    private NavmeshMovementAgent _navmeshAgent;
    private SearchForTarget _searchForTarget;

    private Dictionary<ActivityType, VillagerState> _states = new Dictionary<ActivityType, VillagerState>();
    
    private VillagerStateFactory _stateFactory;
    private BuildingStorage _buildingStorage;
    private BuildingPlanner _buildingPlanner;
    
    private Transform _villageCenter;
    private GameTimer _gameTimer;
    private Collider _discoveryCollider;
    private VillagerSystem  _villagerSystem;
    private DialogueSystem _dialogueSystem;
    private VillagerData _villagerData;

    public VillagerStateMachine(Villager villager, NavmeshMovementAgent navmeshAgent, SearchForTarget searchForTarget, 
        SkinReferencesResolver skinReferencesResolver, BuildingStorage buildingStorage, BuildingPlanner buildingPlanner, Transform villageCenter,
        GameTimer gameTimer, Collider discoveryCollider, VillagerSystem villagerSystem, DialogueSystem dialogueSystem)
    {
        _navmeshAgent = navmeshAgent;
        _searchForTarget = searchForTarget;
        _stateFactory = new VillagerStateFactory();
        
        _buildingStorage = buildingStorage;
        _buildingPlanner = buildingPlanner;
        
        _villageCenter = villageCenter;
        _gameTimer = gameTimer;
        _discoveryCollider = discoveryCollider;
        _villagerSystem = villagerSystem;
        _dialogueSystem = dialogueSystem;
        
        SetStates(villager, skinReferencesResolver);
    }

    public void SetStates(Villager villager, SkinReferencesResolver skinReferencesResolver)
    {
        _villagerData = villager.VillagerData;
        _stateFactory.SetParams(_navmeshAgent, villager, villager.VillagerData, _searchForTarget, skinReferencesResolver, _buildingStorage, _buildingPlanner,
            _villageCenter, _gameTimer, _discoveryCollider, _villagerSystem, _dialogueSystem);
        _states.Clear();
        
        _states.Add(ActivityType.Sleep, _stateFactory.CreateSleepState());
        _states.Add(ActivityType.Work, _stateFactory.CreateWorkState());
        _states.Add(ActivityType.Relax, _stateFactory.CreateRelaxState());
        _states.Add(ActivityType.Guard, _stateFactory.CreateGuardState());
    }

    public async UniTask UpdateCurrentState(ActivityType activityType, CancellationToken token)
    {
        await ExitCurrentState(token);
        CurrentState = _states[activityType];
        CurrentState.EnterState();
    }

    public async UniTask ExitCurrentState(CancellationToken token)
    {
        try
        {
            if (CurrentState != null) await CurrentState.ExitState(token);
            CurrentState = null;
            if (_villagerData.IsTalking) await UniTask.WaitWhile(() => _villagerData.IsTalking, cancellationToken: token); 
        }
        catch (OperationCanceledException e)
        {
            Debug.LogWarning(e.Message);
        }
    }
    
    public void Update()
    {
        if (CurrentState is IUpdatableState updatableState) updatableState.Update();
    }

    public void OnDeath()
    {
        CurrentState = null;
    }
    
    public void Dispose()
    {
        foreach (var state in _states.Values)
        {
            state.Dispose();
        }
    }
}