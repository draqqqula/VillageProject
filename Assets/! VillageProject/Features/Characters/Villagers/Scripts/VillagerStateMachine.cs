using System;
using System.Collections.Generic;

public class VillagerStateMachine : IDisposable
{
    public VillagerState CurrentState { get; private set; }
    private VillagerData _villagerData;
    
    private Dictionary<ActivityType, VillagerState> _states = new Dictionary<ActivityType, VillagerState>();

    public VillagerStateMachine(VillagerData villagerData, NavmeshMovementAgent navmeshAgent)
    {
        _states.Add(ActivityType.Sleep, new SleepVillagerState(navmeshAgent, villagerData.HomePoint.DoorPoint));
        _states.Add(ActivityType.Work, new WorkVillagerState(navmeshAgent, villagerData.Profession));
        _states.Add(ActivityType.Relax, new RelaxVillagerState(navmeshAgent, villagerData.HomePoint.RelaxPoint));
        _states.Add(ActivityType.Guard, new GuardVillagerState());
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