using System;
using R3;
using UnityEngine;

[Serializable]
public class BuildingData
{
    [field: SerializeField] public BuildingType Type { get; private set; }
    [field: SerializeField] public State CurrentState { get; set; }
    [field: SerializeField] public int RepairingHours { get; private set; }
    [field: SerializeField] public string RoadIndex { get; set; }
    
    [field: SerializeField] public Transform EnterPoint { get; private set; }
    public ReactiveProperty<BuildingPlan> Plan => _currentPlan;
    private ReactiveProperty<BuildingPlan> _currentPlan = new ReactiveProperty<BuildingPlan>();
    
    public enum State { Wait, Ready, Broken, Build, Reserved}
}

[Serializable]
public class ArcherTowerData : BuildingData
{
    [field: SerializeField] public Transform ArcherPoint { get; private set; }
    [field: SerializeField] public TowerDamageOverTime DamageHandler { get; private set; }
    [field: SerializeField] public AmmunitionStorage AmmunitionStorage { get; private set; }
    [field: SerializeField] public float DamageMultiplier { get; set; }
}

[Serializable]
public class WorkBuildingData : BuildingData
{
    [field: SerializeField] public Transform WorkPoint { get; private set; }
    [field: SerializeField] public Transform RelaxPoint { get; private set; }
}

public enum BuildingType
{
    Gates, ArcherTower, Smoke, Blacksmith, Hospital, Church
}