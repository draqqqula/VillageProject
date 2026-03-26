using System;
using System.Collections.Generic;
using System.Linq;

public class BuildingStorage : IDisposable
{
    private List<Building> _buildings = new List<Building>();
    public event Action<Building> OnBuildingBroken;

    public void Add(Building building)
    {
        _buildings.Add(building);
        building.OnBroken += OnBroken;
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

    private void OnBroken(Building building)
    {
        OnBuildingBroken?.Invoke(building);
    }

    public void Dispose()
    {
        foreach (var building in _buildings)
        {
            building.OnBroken -= OnBroken;
        }
    }
}