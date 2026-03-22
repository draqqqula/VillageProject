using System;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(NavmeshMovementAgent))]
public class Villager : MonoBehaviour
{
    [SerializeField] private VillagerData _villagerData;
    [SerializeField] private ActivityType _currentActivity;
    public VillagerData VillagerData {get; private set;}
    
    [SerializeField] private NavmeshMovementAgent _navmeshAgent;
    private VillagerStateMachine _stateMachine;
    
    [SerializeField] private SearchForTarget _searchForTarget;
    [SerializeField] private Animator _animator;
    
    [Inject] private BuildingStorage _buildingStorage;
    [Inject] private BuildingPlanner _buildingPlanner;

    public void Init(HomeService homeService, Transform villagerCenter, GameTimer gameTimer)
    {
        VillagerData = ScriptableObject.Instantiate(_villagerData);
        
        var home = homeService.OccupyHouse();
        VillagerData.HomePoint = home;
        
        _stateMachine = new VillagerStateMachine(VillagerData, _navmeshAgent, _searchForTarget, _animator, _buildingStorage, _buildingPlanner,
            villagerCenter, gameTimer);
    }

    private void Update()
    {
        _stateMachine?.Update();
    }

    public void ChangeActivity(ActivityType activity)
    {
        if (VillagerData.ActivityType == activity) return;
        
        _stateMachine.UpdateCurrentState(activity);
        
        _currentActivity = _stateMachine.CurrentState.ActivityType;
        VillagerData.ActivityType = _currentActivity;
        Debug.Log($"Villager {gameObject.name} change to {activity}");
    }

    public void SwitchProfession(ProfessionType profession)
    {
        VillagerData.Profession = new Profession() {Type = profession};
        _stateMachine.SetStates(VillagerData);
        _stateMachine.UpdateCurrentState(VillagerData.ActivityType);
        Debug.Log($"Villager {gameObject.name} profession change to {profession}");
    }

    private void OnDestroy()
    {
        _stateMachine.Dispose();
    }
}