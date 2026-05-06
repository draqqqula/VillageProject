using System;
using System.Threading;
using R3;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Random = System.Random;

[RequireComponent(typeof(NavmeshMovementAgent))]
[RequireComponent(typeof(VelocityToAnimation))]
public class Villager : MonoBehaviour, IInteractable, IDialogueTarget
{
    private static readonly int PROFESSION = Animator.StringToHash("Profession");
    private const float TalkAnimationChance = 0.3f;
    private const float InjuredHealthPercentage = 0.3f;
    
    [SerializeField] private VillagerData _villagerData;
    [SerializeField] private ActivityType _currentActivity;
    [SerializeField] private RelaxVillagerStateConfigs _relaxStateConfigs;
    private RelaxVillagerStateConfigs _relaxStateConfigsInstance;
    
    public VillagerData VillagerData {get; private set;}
    
    [SerializeField] private NavmeshMovementAgent _navmeshAgent;
    private VillagerStateMachine _stateMachine;
    
    [SerializeField] private VillagersSkinsInfo _villagersSkinsInfo;
    [SerializeField] private Transform _viewParent;
    private GameObject _currentSkin;
    private SkinChanger _skinChanger;
    
    [SerializeField] private Transform _dialoguePoint;
    [SerializeField] private DialogueIcon _dialogueIconPrefab;
    private DialogueIcon _dialogueIcon;
    
    [Inject] private DialogueView _dialogueWindow;
    [Inject(Id = "Dialogue")] private Canvas _dialogueCanvas;
    
    [SerializeField] private InteractTrigger _interactTrigger;
    private VillagerInteractHandler _interactHandler;
    
    public event Action<Profession> OnProfessionChanged;
    
    public ReadOnlyReactiveProperty<SkinReferencesResolver> SkinReferencesResolver => _skinReferencesResolver;
    private ReactiveProperty<SkinReferencesResolver> _skinReferencesResolver;
    
    [Inject] private Health _health;
    [Inject] private DiContainer _diContainer;
    
    [SerializeField] private DeathEvent _deathEvent;
    
    // tests
    [SerializeField] private float experience;
    [SerializeField] private float loyalty;
    [SerializeField] private float health;
    
    public void Init(HomeService homeService)
    {
        VillagerData = ScriptableObject.Instantiate(_villagerData);
        VillagerData.Init(_navmeshAgent, _health);
        
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
        
        _relaxStateConfigsInstance = ScriptableObject.Instantiate(_relaxStateConfigs);
        _stateMachine = new VillagerStateMachine(this, _navmeshAgent, _relaxStateConfigsInstance, _diContainer);

        _deathEvent.FiredEvent += OnDeath;
        
        _dialogueIcon = Instantiate(_dialogueIconPrefab, _dialogueCanvas.transform);
        _dialogueIcon.transform.position = _dialoguePoint.position;
        _dialogueIcon.Init(_dialoguePoint);

        _interactHandler = new VillagerInteractHandler(this, _interactTrigger, _diContainer);
        VillagerData.Health.AmountReactive.Subscribe(OnHealthChanged).AddTo(this);

        // tests
        VillagerData.Profession.Experience.Subscribe(v => experience = v).AddTo(this); 
        VillagerData.Loyalty.Property.Subscribe(v => loyalty = v).AddTo(this);
        VillagerData.Health.AmountReactive.Subscribe(v => health = v).AddTo(this);
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

    public void SwitchProfession(Profession profession)
    {
        _ = SwitchProfession(profession, gameObject.GetCancellationTokenOnDestroy());
    }

    private async UniTask SwitchProfession(Profession profession, CancellationToken token)
    {
        await _stateMachine.ExitCurrentState(token);
        VillagerData.Profession = profession;
        ChangeSkin();
        
        _skinReferencesResolver.Value.Animator.SetInteger(PROFESSION, (int)profession.Type);
        _stateMachine.SetStates(this);
        if (VillagerData.ActivityType != null)
        {
            _ = _stateMachine.UpdateCurrentState(VillagerData.ActivityType.Value, gameObject.GetCancellationTokenOnDestroy());
        }
        
        OnProfessionChanged?.Invoke(profession);
        Debug.Log($"Villager {gameObject.name} profession change to {profession}");
    }

    public void Speak(string text)
    {
        bool isPlayAnimation = UnityEngine.Random.Range(0f, 1f) <= TalkAnimationChance;
        if (isPlayAnimation) _skinReferencesResolver.CurrentValue.Animator.SetTrigger("Talk");
        Debug.Log($"{gameObject.name} : {text}");
        
        _dialogueIcon.ShowView();
        _dialogueWindow.TrySetText(_villagerData.Key, _villagerData.NameInRussian, _villagerData.Profession.TypeInRussian, text);
    }

    public void KeepSilent()
    {
        _skinReferencesResolver.CurrentValue.Animator.ResetTrigger("Talk");
        _dialogueIcon.HideView();
        _dialogueWindow.HideText(_villagerData.Key);
    }
    
    public void Interact()
    {
        _interactHandler?.Interact();
    }
    
    private void OnHealthChanged(float health)
    {
        if (health <= InjuredHealthPercentage * VillagerData.Health.MaxHealth)
        {
            _skinReferencesResolver.Value.Animator.SetBool("Injured", true);
        }
        else
        {
            _skinReferencesResolver.Value.Animator.SetBool("Injured", false);
        }
    }

    private void OnDeath()
    {
        _stateMachine.OnDeath();
        _skinReferencesResolver.Value.Animator.SetTrigger("Death");
    }

    private void OnDestroy()
    {
        _stateMachine.Dispose();
        _interactHandler?.Dispose();
        _deathEvent.FiredEvent -= OnDeath;
    }
}