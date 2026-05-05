using System;
using R3;
using UnityEngine;

public abstract class ProfessionData : ScriptableObject
{
    [field: SerializeField] public ProfessionType Type {get; set;}
    [field: SerializeField] public int HoursForMaxExperience {get; private set;}
}

public enum ProfessionType
{
    Blacksmith, Armorer, Builder, Archer, Defender, None
}