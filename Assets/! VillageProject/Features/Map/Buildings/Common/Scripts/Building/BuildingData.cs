using System;
using UnityEngine;

[Serializable]
public class BuildingData
{
    [field: SerializeField] public BuildingType Type { get; private set; }
    [field: SerializeField] public State CurrentState { get; set; }
    
    [field: SerializeField] public Transform EnterPoint { get; private set; }
    
    public enum State { Wait, Ready, Broken, Building }
}

[Serializable]
public class ArcherTowerData : BuildingData
{
    [field: SerializeField] public Transform ArcherPoint { get; private set; }
}

public enum BuildingType
{
    Gates, ArcherTower, Smoke, Blacksmith, Hospital, Church
}