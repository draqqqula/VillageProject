using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleController : MonoBehaviour
{
    [SerializeField] private Schedule[] _schedules;
    private List<Schedule> _schedulesInstances;
    
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private VillagerSystem _villagerSystem;
    
    [SerializeField] private ScheduleView _scheduleView;

    private void Awake()
    {
        CreateInstances();
        _gameTimer.OnHourChanged += OnHourChanged;
        _scheduleView.OnPeriodChanged += OnPeriodChanged;
        _scheduleView.OnSavedSchedule += OnSavedSchedule;
    }

    private void CreateInstances()
    {
        _schedulesInstances = new List<Schedule>();
        foreach (var schedule in _schedules)
        {
            var instance = ScriptableObject.Instantiate(schedule);
            _schedulesInstances.Add(instance);
        }
    }
    
    private void Start()
    {
        _scheduleView.UpdateAllSchedulesView(_schedulesInstances);
        UpdateActivitiesByHour(_gameTimer.CurrentHour);
    }
    
    private void OnHourChanged(int hour)
    {
        UpdateActivitiesByHour(hour);
    }

    private void UpdateActivitiesByHour(int hour)
    {
        foreach (var schedule in _schedulesInstances)
        {
            foreach (var period in schedule.SchedulePeriods)
            {
                if (period.EndTime == period.StartTime)
                {
                    UpdateActivity(schedule, period);
                }
                else if (period.EndTime < period.StartTime)
                {
                    if ((period.StartTime <= hour && hour < 24) || (hour >= 0 && hour < period.EndTime))
                    {
                        UpdateActivity(schedule, period);
                    }
                }
                else
                {
                    if (period.StartTime <= hour && hour < period.EndTime)
                    {
                        UpdateActivity(schedule, period);
                    } 
                }
            }
        }
    }

    private void UpdateActivity(Schedule schedule, SchedulePeriod period)
    {
        var villager = _villagerSystem.GetVillager(schedule.VillagerKey);
        if (villager != null) villager.ChangeActivity(period.ActivityType);
    }

    public void UpdateActivity(string villagerKey, ActivityType activityType, int startTime, int endTime)
    {
        var schedule = _schedulesInstances.FirstOrDefault(schedule => schedule.VillagerKey == villagerKey);
        if (schedule == null) return;

        var length = endTime != startTime ? (endTime - startTime + 24) % 24 : 24;

        for (int i = 0; i < length; i += 1)
        {
            ChangePeriod(schedule, ConvertToHoursFormat(startTime + i), activityType);
        }
        _scheduleView.UpdateView(schedule);
    }

    private void OnPeriodChanged(string villagerKey, int period, ActivityColorData data)
    {
        var schedule = _schedulesInstances.FirstOrDefault(schedule => schedule.VillagerKey == villagerKey);
        if (schedule == null) return;
        ChangePeriod(schedule, period, data.ActivityType);
    }

    private void ChangePeriod(Schedule schedule, int period, ActivityType activityType)
    {
        var foundedPeriod = schedule.GetPeriod(period);
        SplitPeriod(foundedPeriod, period, schedule);
        schedule.SchedulePeriods.Add(new SchedulePeriod() {ActivityType = activityType, StartTime = period, 
            EndTime = ConvertToHoursFormat(period + 1)});
        
        schedule.MergePeriods();
        UpdateActivitiesByHour(_gameTimer.CurrentHour);
    }

    private void OnSavedSchedule(string scheduleKey)
    {
        var schedule = _schedulesInstances.FirstOrDefault(schedule => schedule.VillagerKey == scheduleKey);
        if (schedule == null) return;

        int scheduleIndex = -1;
        for (int i = 0; i < _schedules.Length; i++)
        {
            if (_schedules[i].VillagerKey == scheduleKey) scheduleIndex = i;
        }

        if (scheduleIndex >= 0)
        {
            var newSchedule = _schedules[scheduleIndex];
            newSchedule.SchedulePeriods = new List<SchedulePeriod>();
            foreach (var period in schedule.SchedulePeriods)
            {
                newSchedule.SchedulePeriods.Add(new SchedulePeriod() {StartTime = period.StartTime, EndTime = period.EndTime, 
                    ActivityType = period.ActivityType});
            }
            newSchedule.VillagerKey = schedule.VillagerKey;
        }
    }

    private void SplitPeriod(SchedulePeriod foundedPeriod, int period, Schedule schedule)
    {
        if (foundedPeriod.Length == 24)
        {
            foundedPeriod.StartTime = ConvertToHoursFormat(period + 1);
            foundedPeriod.EndTime = period;
        }
        
        if (foundedPeriod.StartTime == period)
        {
            var length = foundedPeriod.Length;
            foundedPeriod.StartTime = ConvertToHoursFormat(foundedPeriod.StartTime + 1);
            if (length - 1 <= 0) schedule.SchedulePeriods.Remove(foundedPeriod);
        }
        else if (ConvertToHoursFormat(foundedPeriod.EndTime - 1) == period)
        {
            var length = foundedPeriod.Length;
            foundedPeriod.EndTime = ConvertToHoursFormat(foundedPeriod.EndTime - 1);
            if (length - 1 <= 0) schedule.SchedulePeriods.Remove(foundedPeriod);
        }
        else
        {
            var newPeriod = new SchedulePeriod() {ActivityType = foundedPeriod.ActivityType, StartTime = ConvertToHoursFormat(period + 1),
                EndTime = foundedPeriod.EndTime};
            
            schedule.SchedulePeriods.Add(newPeriod);
            foundedPeriod.EndTime = period;
        }
    }

    private int ConvertToHoursFormat(int hour)
    {
        return (hour + 24) % 24;
    }
    
    private void OnDestroy()
    {
        _gameTimer.OnHourChanged -= OnHourChanged;
        _scheduleView.OnPeriodChanged -= OnPeriodChanged;
        _scheduleView.OnSavedSchedule -= OnSavedSchedule;
    }
}