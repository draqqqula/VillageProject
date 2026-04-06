using System;
using System.Threading;
using R3;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(NavmeshMovementAgent))]
[RequireComponent(typeof(VelocityToAnimation))]
public class Villager : MonoBehaviour
{
    private static readonly int PROFESSION = Animator.StringToHash("Profession");
    
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
        
        _skinReferencesResolver.Value.Animator.SetInteger(PROFESSION, (int)VillagerData.Profession.Type);
        if (TryGetComponent(out VelocityToAnimation velocityToAnimation))
        {
            velocityToAnimation.SetReferencesResolver(_skinReferencesResolver);
        }
        
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
        
        _ = _stateMachine.UpdateCurrentState(activity, gameObject.GetCancellationTokenOnDestroy());
        
        _currentActivity = activity;
        VillagerData.ActivityType = _currentActivity;
        Debug.Log($"Villager {gameObject.name} change to {activity}");
    }

    public void SwitchProfession(ProfessionType profession)
    {
        _ = SwitchProfession(profession, gameObject.GetCancellationTokenOnDestroy());
    }

    private async UniTask SwitchProfession(ProfessionType profession, CancellationToken token)
    {
        await _stateMachine.ExitCurrentState(token);
        
        Debug.Log($"Villager {gameObject.name} profession change to {profession}");
        VillagerData.Profession = new Profession() {Type = profession};
        ChangeSkin();
        
        _skinReferencesResolver.Value.Animator.SetInteger(PROFESSION, (int)profession);
        _stateMachine.SetStates(VillagerData, _skinReferencesResolver.Value);
        _ = _stateMachine.UpdateCurrentState(VillagerData.ActivityType, gameObject.GetCancellationTokenOnDestroy());
        Debug.Log($"Villager {gameObject.name} profession change to {profession}");
    }

    private void OnDestroy()
    {
        _stateMachine.Dispose();
    }
}