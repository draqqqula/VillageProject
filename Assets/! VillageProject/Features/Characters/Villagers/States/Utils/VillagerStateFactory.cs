using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VillagerStateFactory
{
    private const float DialogueChance = 0.3f;
    
    private NavmeshMovementAgent _navmeshAgent;
    
    private Villager _villager;
    private VillagerData _villagerData;
    private SearchForTarget _searchForTarget;
    private SkinReferencesResolver _skinReferencesResolver;
    
    private BuildingStorage _buildingStorage;
    private BuildingPlanner _buildingPlanner;
    
    private Transform _villageCenter;
    private GameTimer _gameTimer;
    private Collider _discoveryCollider;
    private VillagerSystem _villagerSystem;
    private DialogueSystem _dialogueSystem;
    
    public void SetParams(NavmeshMovementAgent navmeshAgent, Villager villager, VillagerData villagerData, SearchForTarget searchForTarget,
        SkinReferencesResolver skinReferencesResolver, BuildingStorage buildingStorage, BuildingPlanner buildingPlanner,
        Transform villageCenter, GameTimer gameTimer, Collider discoveryCollider, VillagerSystem villagerSystem, DialogueSystem dialogueSystem)
    {
        _navmeshAgent = navmeshAgent;
        _villager = villager;
        _villagerData = villagerData;
        _searchForTarget = searchForTarget;
        _skinReferencesResolver = skinReferencesResolver;
        
        _buildingStorage = buildingStorage;
        _buildingPlanner = buildingPlanner;
        
        _villageCenter = villageCenter;
        _gameTimer = gameTimer;
        _discoveryCollider = discoveryCollider;
        _villagerSystem = villagerSystem;
        _dialogueSystem = dialogueSystem;
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
            return new DefenderVillagerGuardState(_navmeshAgent, _searchForTarget, _skinReferencesResolver.Animator);
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
                return new BlacksmithWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage);
            case (ProfessionType.Builder):
                return new BuilderWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage, 
                    _buildingPlanner, _gameTimer);
            case (ProfessionType.Armorer):
                return new ArmorerWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage);
            case (ProfessionType.Archer):
                return new ArcherWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage,
                    _searchForTarget, _discoveryCollider, this);
            case (ProfessionType.Defender):
                return new DefenderWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _villageCenter);
            default:
                throw new ArgumentException($"{_villagerData.Profession.Type} is not a valid profession!");
        }
    }

    public DefenderWorkState CreateDefenderWorkState()
    {
        return new DefenderWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _villageCenter);
    }
}