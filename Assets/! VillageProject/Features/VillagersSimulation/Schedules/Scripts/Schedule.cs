using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Schedule", menuName = "Villagers Simulation/New Schedule")]
public class Schedule : ScriptableObject
{
    [field: SerializeField] public string VillagerKey {get; set;}
    [field: SerializeField] public List<SchedulePeriod> SchedulePeriods {get; set;}

    public SchedulePeriod GetPeriod(int hour)
    {
        foreach (var period in SchedulePeriods)
        {
            if (period.EndTime == period.StartTime)
            {
                return period;
            }
            
            if (period.EndTime < period.StartTime)
            {
                if ((period.StartTime <= hour && hour <= 23) || (hour < period.EndTime && hour >= 0)) return period;
            }
            else
            {
                if (period.StartTime <= hour && hour < period.EndTime) return period;
            }
        }
        
        throw new ArgumentOutOfRangeException($"Period {hour} out of range!");
    }

    [ContextMenu("Merge Periods")]
    public void MergePeriods()
    {
        var sortedPeriods = SchedulePeriods.OrderBy(period => period.StartTime).ToList();
        var merged = new List<SchedulePeriod>();

        foreach (var period in sortedPeriods)
        {
            if (merged.Count == 0)
            {
                merged.Add(period);
                continue;
            }
            
            MergePeriods( merged.Last(), period, merged);
        }
        
        MergePeriods(merged.Last(),  merged.First(), merged);
        if (merged.Count > 1 && merged.First().ActivityType == merged.Last().ActivityType) merged.RemoveAt(0); 
        
        SchedulePeriods = merged.OrderBy(period => period.StartTime).ToList();
    }

    private void MergePeriods(SchedulePeriod period1, SchedulePeriod period2, List<SchedulePeriod> merged)
    {
        if (period1.ActivityType == period2.ActivityType)
        {
            period1.EndTime = period2.EndTime;
        }
        else
        {
            merged.Add(period2);
        }
    }
}

[Serializable]
public class SchedulePeriod
{
    [field: SerializeField] public int StartTime {get; set;}
    [field: SerializeField] public int EndTime {get; set;}
    public int Length => EndTime != StartTime ? (EndTime - StartTime + 24) % 24 : 24;
    [field: SerializeField] public ActivityType ActivityType {get; set;}
}

public enum ActivityType
{
    Sleep, Work, Relax, Guard
}