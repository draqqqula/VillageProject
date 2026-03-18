using System;

public class VillagerStateFactory
{
    private NavmeshMovementAgent _navmeshAgent;
    private VillagerData _villagerData;
    private BuildingStorage _buildingStorage;
    
    public void SetParams(NavmeshMovementAgent navmeshAgent, VillagerData villagerData, BuildingStorage buildingStorage)
    {
        _navmeshAgent = navmeshAgent;
        _villagerData = villagerData;
        _buildingStorage = buildingStorage;
    }
    
    public SleepVillagerState CreateSleepState()
    {
        return new SleepVillagerState(_navmeshAgent, _villagerData.HomePoint.DoorPoint);
    }

    public RelaxVillagerState CreateRelaxState()
    {
        return new RelaxVillagerState(_navmeshAgent, _villagerData.HomePoint.RelaxPoint);
    }

    public GuardVillagerState CreateGuardState()
    {
        return new GuardVillagerState();
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
                return new DefenderWorkState(_navmeshAgent, _villagerData.Profession);
            default:
                throw new ArgumentException($"{_villagerData.Profession.Type} is not a valid profession!");
        }
    }
}