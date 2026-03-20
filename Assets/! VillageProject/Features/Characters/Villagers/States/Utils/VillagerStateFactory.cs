using System;
using UnityEngine;

public class VillagerStateFactory
{
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerData _villagerData;
    private BuildingStorage _buildingStorage;
    private Transform _villageCenter;
    private SearchForTarget _searchForTarget;
    private Animator _animator;
    
    public void SetParams(NavmeshMovementAgent navmeshAgent, VillagerData villagerData, BuildingStorage buildingStorage,
        Transform villageCenter, SearchForTarget searchForTarget, Animator animator)
    {
        _navmeshAgent = navmeshAgent;
        _villagerData = villagerData;
        _buildingStorage = buildingStorage;
        _villageCenter = villageCenter;
        _searchForTarget = searchForTarget;
        _animator = animator;
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
            return new DefenderVillagerGuardState(_navmeshAgent, _searchForTarget, _animator);
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
                return new BlacksmithWorkState(_navmeshAgent, _villagerData.Profession, _buildingStorage);
            case (ProfessionType.Builder):
                return new BuilderWorkState(_navmeshAgent, _villagerData.Profession, _buildingStorage);
            case (ProfessionType.Armorer):
                return new ArmorerWorkState(_navmeshAgent, _villagerData.Profession, _buildingStorage);
            case (ProfessionType.Archer):
                return new ArcherWorkState(_navmeshAgent, _villagerData.Profession, _buildingStorage);
            case (ProfessionType.Defender):
                return new DefenderWorkState(_navmeshAgent, _villagerData.Profession, _villageCenter);
            default:
                throw new ArgumentException($"{_villagerData.Profession.Type} is not a valid profession!");
        }
    }
}