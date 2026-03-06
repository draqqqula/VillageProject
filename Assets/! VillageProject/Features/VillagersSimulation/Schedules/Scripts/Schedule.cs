using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Schedule", menuName = "Villagers Simulation/New Schedule")]
public class Schedule : ScriptableObject
{
    [field: SerializeField] public string VillagerKey {get; set;}
    [field: SerializeField] public SchedulePeriod[] SchedulePeriods {get; set;}
}

[Serializable]
public class SchedulePeriod
{
    [field: SerializeField] public int StartTime {get; set;}
    [field: SerializeField] public int EndTime {get; set;}
    [field: SerializeField] public ActivityType ActivityType {get; set;}
}

public enum ActivityType
{
    Sleep, Work, Relax, Guard
}