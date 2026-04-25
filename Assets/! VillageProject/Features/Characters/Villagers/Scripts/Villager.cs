using System;
using System.Threading;
using R3;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Cysharp.Threading.Tasks;
using TMPro;

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

    [SerializeField] private Collider _discoveryCollider;
    [SerializeField] private Transform _dialoguePoint;
    [SerializeField] private DialogueView _dialogueWindowPrefab;
    private DialogueView _dialogueWindow;
    [SerializeField] private Canvas _dialogueCanvas;
    
    public ReadOnlyReactiveProperty<SkinReferencesResolver> SkinReferencesResolver => _skinReferencesResolver;
    private ReactiveProperty<SkinReferencesResolver> _skinReferencesResolver;
    
    [Inject] private BuildingStorage _buildingStorage;
    [Inject] private BuildingPlanner _buildingPlanner;
    [Inject] private DiContainer _diContainer;
    [Inject] private DialogueSystem _dialogueSystem;
    
    [SerializeField] private DeathEvent _deathEvent;
    
    public void Init(HomeService homeService, Transform villagerCenter, GameTimer gameTimer, VillagerSystem villagerSystem)
    {
        VillagerData = ScriptableObject.Instantiate(_villagerData);
        VillagerData.NavmeshAgent = _navmeshAgent;
        
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
        
        _stateMachine = new VillagerStateMachine(this, _navmeshAgent, _searchForTarget, _skinReferencesResolver.Value,
            _buildingStorage, _buildingPlanner, villagerCenter, gameTimer, _discoveryCollider, villagerSystem, _dialogueSystem);

        _deathEvent.FiredEvent += OnDeath;
        
        _dialogueWindow = Instantiate(_dialogueWindowPrefab, _dialogueCanvas.transform);
        _dialogueWindow.transform.position = _dialoguePoint.position;
        _dialogueWindow.Init(_dialoguePoint);
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
        if (VillagerData.ActivityType != null && VillagerData.ActivityType.Value == activity) return;
        
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
        VillagerData.Profession = new Profession() {Type = profession};
        ChangeSkin();
        
        _skinReferencesResolver.Value.Animator.SetInteger(PROFESSION, (int)profession);
        _stateMachine.SetStates(this, _skinReferencesResolver.Value);
        if (VillagerData.ActivityType != null)
        {
            _ = _stateMachine.UpdateCurrentState(VillagerData.ActivityType.Value, gameObject.GetCancellationTokenOnDestroy());
        }
        Debug.Log($"Villager {gameObject.name} profession change to {profession}");
    }

    public void Speak(string text)
    {
        Debug.Log($"{gameObject.name} : {text}");
        _dialogueWindow.ShowView(text);
    }

    public void KeepSilent()
    {
        _dialogueWindow.HideView();
    }

    private void OnDeath()
    {
        _stateMachine.OnDeath();
        _skinReferencesResolver.Value.Animator.SetTrigger("Death");
    }

    private void OnDestroy()
    {
        _stateMachine.Dispose();
        _deathEvent.FiredEvent -= OnDeath;
    }
}