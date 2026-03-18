using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavmeshMovementAgent))]
public class Villager : MonoBehaviour
{
    [SerializeField] private VillagerData _villagerData;
    [SerializeField] private ActivityType _currentActivity;
    public VillagerData VillagerData {get; private set;}
    
    [SerializeField] private NavmeshMovementAgent _navmeshAgent;
    private VillagerStateMachine _stateMachine;

    public void Init(ProfessionService professionService, HomeService homeService)
    {
        VillagerData = ScriptableObject.Instantiate(_villagerData);
        
        var profession = professionService.GetProfession(VillagerData.ProfessionType);
        VillagerData.Profession = profession;

        var home = homeService.OccupyHouse();
        VillagerData.HomePoint = home;
        
        _stateMachine = new VillagerStateMachine(VillagerData, _navmeshAgent);
    }
    
    public void ChangeActivity(ActivityType activity)
    {
        if (VillagerData.ActivityType == activity) return;
        
        _stateMachine.UpdateCurrentState(activity);
        
        _currentActivity = _stateMachine.CurrentState.ActivityType;
        VillagerData.ActivityType = _currentActivity;
        Debug.Log($"Villager {gameObject.name} change to {activity}");
    }

    private void OnDestroy()
    {
        _stateMachine.Dispose();
    }
}