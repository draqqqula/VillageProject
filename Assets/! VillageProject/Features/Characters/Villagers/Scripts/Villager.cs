using System;
using R3;
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
    [SerializeField] private VillagersSkinsInfo _villagersSkinsInfo;
    [SerializeField] private Transform _viewParent;
    private GameObject _currentSkin;
    private SkinChanger _skinChanger;
    
    public ReadOnlyReactiveProperty<SkinReferencesResolver> SkinReferencesResolver => _skinReferencesResolver;
    private ReactiveProperty<SkinReferencesResolver> _skinReferencesResolver;
    
    [Inject] private BuildingStorage _buildingStorage;
    [Inject] private BuildingPlanner _buildingPlanner;
    [Inject] private DiContainer _diContainer;
    
    public void Init(HomeService homeService, Transform villagerCenter, GameTimer gameTimer)
    {
        VillagerData = ScriptableObject.Instantiate(_villagerData);
        
        var skinsInfoInstance = ScriptableObject.Instantiate(_villagersSkinsInfo);
        _skinChanger = new SkinChanger(skinsInfoInstance, _diContainer);
        ChangeSkin();
        
        var home = homeService.OccupyHouse();
        VillagerData.HomePoint = home;
        
        _stateMachine = new VillagerStateMachine(VillagerData, _navmeshAgent, _searchForTarget, _skinReferencesResolver.Value,
            _buildingStorage, _buildingPlanner, villagerCenter, gameTimer);
    }

    private void ChangeSkin()
    {
        if (_currentSkin != null) Destroy(_currentSkin);
        
        var newSkin = _skinChanger.CreateSkin(_viewParent, VillagerData.Gender, VillagerData.Profession.Type);
        var skinResolver = newSkin.GetComponent<SkinReferencesResolver>();

        if (_skinReferencesResolver == null) _skinReferencesResolver = new ReactiveProperty<SkinReferencesResolver>(skinResolver);
        else _skinReferencesResolver.Value = skinResolver;
        
        _currentSkin = newSkin;
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
        Debug.Log($"Villager {gameObject.name} profession change to {profession}");
        VillagerData.Profession = new Profession() {Type = profession};
        ChangeSkin();
        
        _stateMachine.SetStates(VillagerData, _skinReferencesResolver.Value);
        _stateMachine.UpdateCurrentState(VillagerData.ActivityType);
        Debug.Log($"Villager {gameObject.name} profession change to {profession}");
    }

    private void OnDestroy()
    {
        _stateMachine.Dispose();
    }
}