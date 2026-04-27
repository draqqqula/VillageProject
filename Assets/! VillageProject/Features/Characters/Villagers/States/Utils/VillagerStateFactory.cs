using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Zenject;

public class VillagerStateFactory
{
    private const float DialogueChance = 0.3f;
    
    private NavmeshMovementAgent _navmeshAgent;
    
    private SearchForTarget _searchForTarget;
    private Collider _discoveryCollider;
    
    private Villager _villager;
    private VillagerData _villagerData;
    private SkinReferencesResolver _skinReferencesResolver;
    
    private BuildingStorage _buildingStorage;
    private BuildingPlanner _buildingPlanner;
    
    private Transform _villageCenter;
    private GameTimer _gameTimer;
    private VillagerSystem _villagerSystem;
    
    private DialogueSystem _dialogueSystem;
    private ProfessionController _professionController;
    
    public void SetParams(NavmeshMovementAgent navmeshAgent, Villager villager, DiContainer container)
    {
        _navmeshAgent = navmeshAgent;
        _villager = villager;
        _villagerData = _villager.VillagerData;
        _skinReferencesResolver = _villager.SkinReferencesResolver.CurrentValue;
        _searchForTarget = container.Resolve<SearchForTarget>();
        
        _buildingStorage = container.Resolve<BuildingStorage>();
        _buildingPlanner = container.Resolve<BuildingPlanner>();
        
        _villageCenter = container.ResolveId<Transform>("VillageCenter");
        _gameTimer = container.Resolve<GameTimer>();
        _discoveryCollider = container.ResolveId<Collider>("Discovery");
        _villagerSystem = container.Resolve<VillagerSystem>();
        _dialogueSystem = container.Resolve<DialogueSystem>();
        _professionController = container.Resolve<ProfessionController>();
    }
    
    public SleepVillagerState CreateSleepState()
    {
        return new SleepVillagerState(_navmeshAgent, _villagerData);
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
                return new IdleRelaxVillagerState(_navmeshAgent, _skinReferencesResolver, _villagerData.HomePoint.RelaxPoint, false);
            }
            else return new IdleRelaxVillagerState(_navmeshAgent, _skinReferencesResolver, workData.RelaxPoint, true);
        }
        else
        {
            bool isTalking = Random.Range(0f, 1f) <= DialogueChance;
            
            if (isTalking) return new TalkRelaxVillagerState(_navmeshAgent, _villager, _villagerSystem, _dialogueSystem, _villageCenter);
            return new IdleRelaxVillagerState(_navmeshAgent, _skinReferencesResolver, _villagerData.HomePoint.RelaxPoint, false);
        }
    }

    public GuardVillagerState CreateGuardState()
    {
        if (_villagerData.Profession.Type == ProfessionType.Defender)
        {
            return new DefenderVillagerGuardState(_navmeshAgent, _searchForTarget, _skinReferencesResolver.Animator, _villagerData.Profession, _gameTimer);
        }
        else
        {
            return new PeacefulVillagerGuardState(_navmeshAgent, _villageCenter, _villagerData);
        }
    }

    public WorkVillagerState CreateWorkState()
    {
        switch (_villagerData.Profession.Type)
        {
            case (ProfessionType.Blacksmith):
                return new BlacksmithWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage, _gameTimer);
            case (ProfessionType.Builder):
                return new BuilderWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingPlanner, _gameTimer);
            case (ProfessionType.Armorer):
                return new ArmorerWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage, _gameTimer,
                    _villagerSystem, _professionController);
            case (ProfessionType.Archer):
                return new ArcherWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage,
                    _searchForTarget, _discoveryCollider, this, _gameTimer);
            case (ProfessionType.Defender):
                return new DefenderWorkState(_navmeshAgent, _skinReferencesResolver, _villageCenter);
            default:
                throw new ArgumentException($"{_villagerData.Profession.Type} is not a valid profession!");
        }
    }

    public DefenderWorkState CreateDefenderWorkState()
    {
        return new DefenderWorkState(_navmeshAgent, _skinReferencesResolver, _villageCenter);
    }
}