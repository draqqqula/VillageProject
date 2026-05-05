using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class VillagerStateMachine : IDisposable
{
    public VillagerState CurrentState { get; private set; }
    private NavmeshMovementAgent _navmeshAgent;

    private Dictionary<ActivityType, VillagerState> _states = new Dictionary<ActivityType, VillagerState>();
    
    private VillagerStateFactory _stateFactory;
    private VillagerData _villagerData;
    private RelaxVillagerStateConfigs _relaxStateConfigs;
    
    private DiContainer _container;

    public VillagerStateMachine(Villager villager, NavmeshMovementAgent navmeshAgent, RelaxVillagerStateConfigs relaxStateConfigs,
        DiContainer container)
    {
        _navmeshAgent = navmeshAgent;
        _stateFactory = new VillagerStateFactory();
        _container = container;
        _relaxStateConfigs = relaxStateConfigs;
        
        SetStates(villager);
    }

    public void SetStates(Villager villager)
    {
        _villagerData = villager.VillagerData;
        _stateFactory.SetParams(_navmeshAgent, villager, _relaxStateConfigs, _container);
        _states.Clear();
        
        _states.Add(ActivityType.Sleep, _stateFactory.CreateSleepState());
        if (villager.VillagerData.Profession.Type != ProfessionType.None) _states.Add(ActivityType.Work, _stateFactory.CreateWorkState());
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