using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VillagerData", menuName = "Villagers Simulation/New Villager Data")]
public class VillagerData : ScriptableObject
{
    [field: SerializeField] public string Key {get; private set;}
    [field: SerializeField] public Gender Gender {get; private set;}
    [field: SerializeField] public ActivityType? ActivityType {get; set;}
    [field: SerializeField] public Profession Profession {get; set;}
    
    public bool IsOnHome { get; set; }
    public HomePoint HomePoint {get; set;}
}

public enum Gender {Male, Female}

[Serializable]
public class Profession
{
    [field: SerializeField] public ProfessionType Type {get; set;}
}

public enum ProfessionType
{
    Blacksmith, Armorer, Builder, Archer, Defender
}