using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VillagerData", menuName = "Villagers Simulation/New Villager Data")]
public class VillagerData : ScriptableObject
{
    [field: SerializeField] public string Key {get; private set;}
    [field: SerializeField] public ActivityType ActivityType {get; set;}
    [field: SerializeField] public ProfessionType ProfessionType {get; private set;}
    
    public HomePoint HomePoint {get; set;}
    public Profession Profession {get; set;}
}

[Serializable]
public class Profession
{
    [field: SerializeField] public ProfessionType Type {get; private set;}
    [field: SerializeField] public Transform WorkPoint {get; private set;}
}

public enum ProfessionType
{
    Blacksmith, Armorer, Builder, Archer, Defender
}