using System;
using UnityEngine;

public class VillagerStateFactory
{
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerData _villagerData;
    private SearchForTarget _searchForTarget;
    private SkinReferencesResolver _skinReferencesResolver;
    
    private BuildingStorage _buildingStorage;
    private BuildingPlanner _buildingPlanner;
    
    private Transform _villageCenter;
    private GameTimer _gameTimer;
    
    public void SetParams(NavmeshMovementAgent navmeshAgent, VillagerData villagerData, SearchForTarget searchForTarget,
        SkinReferencesResolver skinReferencesResolver, BuildingStorage buildingStorage, BuildingPlanner buildingPlanner,
        Transform villageCenter, GameTimer gameTimer)
    {
        _navmeshAgent = navmeshAgent;
        _villagerData = villagerData;
        _searchForTarget = searchForTarget;
        _skinReferencesResolver = skinReferencesResolver;
        
        _buildingStorage = buildingStorage;
        _buildingPlanner = buildingPlanner;
        
        _villageCenter = villageCenter;
        _gameTimer = gameTimer;
    }
    
    public SleepVillagerState CreateSleepState()
    {
        return new SleepVillagerState(_navmeshAgent, _villagerData.HomePoint.DoorPoint, _villagerData);
    }

    public RelaxVillagerState CreateRelaxState()
    {
        return new RelaxVillagerState(_navmeshAgent, _villagerData.HomePoint.RelaxPoint);
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
                return new ArcherWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _buildingStorage);
            case (ProfessionType.Defender):
                return new DefenderWorkState(_navmeshAgent, _skinReferencesResolver, _villagerData.Profession, _villageCenter);
            default:
                throw new ArgumentException($"{_villagerData.Profession.Type} is not a valid profession!");
        }
    }
}