using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Zenject;

public class VillagerStateFactory
{
    private NavmeshMovementAgent _navmeshAgent;
    private AttackBonus _attackBonus;
    
    private SearchForTarget _searchForTarget;
    private Collider _discoveryCollider;
    
    private Villager _villager;
    private VillagerData _villagerData;
    private SkinReferencesResolver _skinReferencesResolver;
    private RelaxVillagerStateConfigs _relaxStatesConfigs;
    
    private BuildingStorage _buildingStorage;
    private BuildingPlanner _buildingPlanner;
    
    private Transform _villageCenter;
    private Transform _villagersSpawnPoint;
    private GameTimer _gameTimer;
    private VillagerSystem _villagerSystem;
    private WaveController _waveController;
    
    private DialogueSystem _dialogueSystem;
    private ProfessionController _professionController;
    private SkipTimeController _skipTimeController;
    private InterestingPointsService _interestingPointsService;
    
    public void SetParams(NavmeshMovementAgent navmeshAgent, Villager villager, RelaxVillagerStateConfigs relaxStateConfigs,
        DiContainer container)
    {
        _navmeshAgent = navmeshAgent;
        _villager = villager;
        _villagerData = _villager.VillagerData;
        _skinReferencesResolver = _villager.SkinReferencesResolver.CurrentValue;
        _relaxStatesConfigs = relaxStateConfigs;
        
        _searchForTarget = container.Resolve<SearchForTarget>();
        _attackBonus = container.Resolve<AttackBonus>();
        
        _buildingStorage = container.Resolve<BuildingStorage>();
        _buildingPlanner = container.Resolve<BuildingPlanner>();
        
        _villageCenter = container.ResolveId<Transform>("VillageCenter");
        _villagersSpawnPoint = container.ResolveId<Transform>("VillagersSpawnPoint");
        
        _gameTimer = container.Resolve<GameTimer>();
        _discoveryCollider = container.ResolveId<Collider>("Discovery");
        _villagerSystem = container.Resolve<VillagerSystem>();
        _waveController = container.Resolve<WaveController>();
        
        _dialogueSystem = container.Resolve<DialogueSystem>();
        _professionController = container.Resolve<ProfessionController>();
        _skipTimeController = container.Resolve<SkipTimeController>();
        _interestingPointsService = container.Resolve<InterestingPointsService>();
    }
    
    public SleepVillagerState CreateSleepState()
    {
        return new SleepVillagerState(_navmeshAgent, _villagerData, _skinReferencesResolver, _skipTimeController);
    }

    public RelaxVillagerState CreateRelaxState()
    {
        if (_villagerData.Profession.Type == ProfessionType.Blacksmith || _villagerData.Profession.Type == ProfessionType.Armorer)
        {
            WorkBuildingData workData = null;

            if (_villagerData.Profession.Type == ProfessionType.Blacksmith)
                workData = _buildingStorage.Get(BuildingType.Blacksmith)?.Data as WorkBuildingData;
            else workData = _buildingStorage.Get(BuildingType.Hospital)?.Data as WorkBuildingData;

            if (workData == null)
            {
                Debug.LogError("Building has not WorkBuildingData!");
                return CreateRelaxInHomeVillagerState();
            }
            else return CreateRelaxInWorkVillagerState(workData.RelaxPoint);
        }
        else
        {
            return new CombineRelaxVillagerState(_navmeshAgent.gameObject, _villagerData, _relaxStatesConfigs, this, _gameTimer, _skipTimeController);
        }
    }

    public GuardVillagerState CreateGuardState()
    {
        if (_villagerData.Profession.Type == ProfessionType.Defender)
        {
            return new DefenderVillagerGuardState(_navmeshAgent, _searchForTarget, _skinReferencesResolver.Animator, _villagerData.Profession, 
                _gameTimer, _attackBonus);
        }
        else
        {
            return new PeacefulVillagerGuardState(_navmeshAgent, _villageCenter, _villagerData, _skinReferencesResolver);
        }
    }

    public WorkVillagerState CreateWorkState()
    {
        switch (_villagerData.Profession.Type)
        {
            case (ProfessionType.Blacksmith):
                return new BlacksmithWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage, _gameTimer,
                    _skipTimeController, _villagerData);
            case (ProfessionType.Builder):
                return new BuilderWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingPlanner, _gameTimer,
                    _skipTimeController, _villagerData, _villagersSpawnPoint);
            case (ProfessionType.Armorer):
                return new ArmorerWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage, _gameTimer,
                    _villagerSystem, _professionController, _skipTimeController, _villagerData);
            case (ProfessionType.Archer):
                return new ArcherWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage, _searchForTarget,
                    _discoveryCollider, this, _gameTimer, _villageCenter, _waveController, _skipTimeController, _villagerData);
            case (ProfessionType.Defender):
                return new DefenderWorkState(_villagerData, _navmeshAgent, _skinReferencesResolver, _villageCenter);
            default:
                throw new ArgumentException($"{_villagerData.Profession.Type} is not a valid profession!");
        }
    }

    public DefenderWorkState CreateDefenderWorkState()
    {
        return new DefenderWorkState(_villagerData, _navmeshAgent, _skinReferencesResolver, _villageCenter);
    }

    public IdleRelaxVillagerState CreateRelaxInHomeVillagerState()
    {
        return new IdleRelaxVillagerState(_navmeshAgent, _skinReferencesResolver, _villagerData.HomePoint.RelaxPoint, false);
    }
    
    public IdleRelaxVillagerState CreateRelaxInWorkVillagerState(Transform relaxPoint)
    {
        return new IdleRelaxVillagerState(_navmeshAgent, _skinReferencesResolver, relaxPoint, true);
    }

    public TalkRelaxVillagerState CreateTalkRelaxVillagerState()
    {
        return new TalkRelaxVillagerState(_navmeshAgent, _villager, _villagerSystem, _dialogueSystem, _villageCenter);
    }

    public WalkInCenterVillagerState CreateWalkInCenterState()
    {
        return new WalkInCenterVillagerState(_villagerData, _navmeshAgent, _skinReferencesResolver, _villageCenter);
    }

    public VisitInterestingPointsState CreateVisitInterestingPointsState()
    {
        return new VisitInterestingPointsState(_villagerData,_navmeshAgent, _skinReferencesResolver, _interestingPointsService,
            _skipTimeController, this);
    }
}