using System.Collections.Generic;
using System.Linq;

public class BuildingStorage
{
    private List<Building> _buildings = new List<Building>();

    public void Add(Building building)
    {
        _buildings.Add(building);
    }

    public Building Get(BuildingType buildingType)
    {
        return _buildings.FirstOrDefault(b => b.Data.Type == buildingType);
    }

    public Building Get(BuildingType buildingType, BuildingData.State state)
    {
        return _buildings.FirstOrDefault(b => b.Data.Type == buildingType && b.Data.CurrentState == state);
    }

    public Building GetBroken()
    {
        return _buildings.FirstOrDefault(b => b.Data.CurrentState == BuildingData.State.Broken);
    }
}