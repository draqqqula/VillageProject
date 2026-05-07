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
            if (IsHourInPeriod(hour, period)) return period;
        }
        
        throw new ArgumentOutOfRangeException($"Period {hour} out of range!");
    }

    public SchedulePeriod GetNextPeriod(SchedulePeriod period)
    {
        var endTime = period.EndTime;
        return GetPeriod(endTime);
    }

    public bool IsHourInPeriod(int hour, SchedulePeriod period)
    {
        return IsHourInRange(period.StartTime, period.EndTime, hour);
    }

    public bool IsPeriodInRange(int startHour, int endHour, SchedulePeriod period)
    {
        return IsHourInRange(startHour, endHour, period.StartTime) || IsHourInRange(startHour, endHour, period.EndTime);
    }
    
    public bool IsHourInRange(int startHour, int endHour, int hour)
    {
        if (startHour == endHour)
        {
            return true;
        }
            
        if (endHour < startHour)
        {
            if ((startHour <= hour && hour <= 23) || (hour < endHour && hour >= 0)) return true;
        }
        else
        {
            if (startHour <= hour && hour < endHour) return true;
        }
        
        return false;
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
            if (period2.Length == 23)
            {
                period2.EndTime = period1.EndTime;
                merged.Remove(period1);
                merged.Add(period2);
                return;
            }
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