using System;
using R3;
using UnityEngine;

[CreateAssetMenu(fileName = "Profession", menuName = "Villagers Simulation/New Profession")]
public class ProfessionData : ScriptableObject
{
    [field: SerializeField] public ProfessionType Type {get; set;}
    [field: SerializeField] public int HoursForMaxExperience {get; private set;}
}

public enum ProfessionType
{
    Blacksmith, Armorer, Builder, Archer, Defender
}