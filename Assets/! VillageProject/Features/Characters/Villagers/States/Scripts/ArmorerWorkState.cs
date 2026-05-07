using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class ArmorerWorkState : WorkVillagerState
{
    private const int MoveHours = 2;
    private const int MinDistanceToBuilding = 2;
    
    private Transform target;
    private Profession _profession;
    private NavmeshMovementAgent _navMeshAgent;
    
    private VillagerTransformHandler _movementHandler;
    private SkinReferencesResolver _skinReferencesResolver;
    private RaiseExperienceHandler _experienceHandler;
    private ProfessionController _professionController;

    private List<Villager> _defenders = new List<Villager>();
    private VillagerSystem _villagerSystem;
    private GameTimer _gameTimer;

    private bool _isWorking;
    private SkipTimeController _skipTimeController;
    
    public ArmorerWorkState(NavmeshMovementAgent navmeshAgent, SkinReferencesResolver skinReferencesResolver,
        Profession profession, BuildingStorage buildingStorage, GameTimer gameTimer, VillagerSystem villagerSystem, 
        ProfessionController professionController, SkipTimeController skipTimeController)
    {
        _navMeshAgent = navmeshAgent;
        _profession = profession;
        
        _skinReferencesResolver = skinReferencesResolver;
        var hospital = buildingStorage.Get(BuildingType.Hospital);
        target = (hospital.Data as WorkBuildingData).WorkPoint;
        
        _villagerSystem = villagerSystem;
        _professionController = professionController;
        _villagerSystem.OnAddedVillager += OnVillagerAdded;
        _professionController.OnProfessionChanged += OnVillagerChangedProfession;
        
        _movementHandler = new VillagerTransformHandler(navmeshAgent);
        _experienceHandler = new RaiseExperienceHandler(profession, gameTimer);
        
        _profession.Experience.Subscribe(TryRaisingArmor).AddTo(navmeshAgent.gameObject);
        _gameTimer = gameTimer;
        _skipTimeController = skipTimeController;
    }
    
    private void InitDefenders()
    {
        _defenders = _villagerSystem.Villagers.Where(v => v.VillagerData.Profession.Type == ProfessionType.Defender).ToList();
    }
    
    private void OnVillagerAdded(Villager villager)
    {
        if (villager.VillagerData.Profession.Type == ProfessionType.Defender)
        {
            _defenders.Add(villager);
        }
    }

    private void OnVillagerChangedProfession(Villager villager, ProfessionType professionType)
    {
        if (professionType == ProfessionType.Defender)
        {
            _defenders.Add(villager);
        }
        else
        {
            if (_defenders.Contains(villager))
            {
                _defenders.Remove(villager);
                
                var healthProgress = villager.VillagerData.Health.Amount / villager.VillagerData.Health.MaxHealth;
                villager.VillagerData.Health.MaxHealth = villager.VillagerData.Health.DefaultMaxHealth;
                villager.VillagerData.Health.Amount = Mathf.Lerp(0, villager.VillagerData.Health.MaxHealth, healthProgress);
            }
        }
    }
    
    public override void EnterState()
    {
        if (_defenders.Count == 0) InitDefenders();
        _movementHandler.ActivateMovementWithRotation(target, callback: OnPointReached);
    }

    public override void EnterStateWithSkip()
    {
        if (_defenders.Count == 0) InitDefenders();
        _navMeshAgent.transform.position = target.position;
        
        if (Vector3.Distance(_navMeshAgent.transform.position, target.position) > MinDistanceToBuilding)
        {
            _experienceHandler.IncreaseHours(MoveHours);
        }
        
        OnPointReached();
    }

    private void OnPointReached()
    {
        _isWorking = true;
        _skinReferencesResolver.Animator.SetBool("Work", true);
        _experienceHandler.StartRaisingExperience();
        
        TryRaisingArmor(_profession.Experience.CurrentValue);
    }

    private void TryRaisingArmor(float experience)
    {
        if (!_isWorking) return;

        var multiplier = (_profession.ProfessionData as ArmorerProfessionData).HealthMultiplierCurve.Evaluate(experience);
        foreach (var defender in _defenders)
        {
            var healthProgress = defender.VillagerData.Health.Amount / defender.VillagerData.Health.MaxHealth;
            defender.VillagerData.Health.MaxHealth = defender.VillagerData.Health.DefaultMaxHealth * multiplier;
            defender.VillagerData.Health.Amount = Mathf.Lerp(0, defender.VillagerData.Health.MaxHealth, healthProgress);
        }
    }

    public override async UniTask ExitState(CancellationToken token)
    {
        _movementHandler.DeactivateMovement();
        
        if (_isWorking)
        {
            _isWorking = false;
            _experienceHandler.StopRaisingExperience();
            
            if (!_skipTimeController.IsSkipping.CurrentValue) 
                await _skinReferencesResolver.AnimatorHandler.TransitByBool("Work", false, token);
            else _skinReferencesResolver.AnimatorHandler.SetBool("Work", false);
        }
    }

    public override void ExitStateWithSkip()
    {
        _ = ExitState(_navMeshAgent.GetCancellationTokenOnDestroy());
    }

    public override void Dispose()
    {
        _movementHandler.Dispose();
        _villagerSystem.OnAddedVillager -= OnVillagerAdded;
        _professionController.OnProfessionChanged -= OnVillagerChangedProfession;
    }
}