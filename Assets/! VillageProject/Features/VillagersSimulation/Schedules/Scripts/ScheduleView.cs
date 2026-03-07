using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleView : MonoBehaviour
{
    [SerializeField] private ActivityColorView _activitiesColorsView;
    
    [SerializeField] private GameTimer _gameTimer;

    [SerializeField] private ScheduleRawView _scheduleRawPrefab;
    [SerializeField] private Transform _schedulesParent;
    private List<ScheduleRawView> _schedulesRaws;

    public event Action<string> OnSavedSchedule;
    public event Action<string, int, ActivityColorData> OnPeriodChanged;
    
    public void UpdateAllView(List<Schedule> schedules)
    {
        if (_schedulesRaws != null) DestroySchedules();
        
        foreach (var schedule in schedules)
        {
            var raw = Instantiate(_scheduleRawPrefab, _schedulesParent);
            raw.Init(_activitiesColorsView);
            raw.OnPeriodChanged += InvokePeriodChangedEvent;
            raw.OnSavedSchedule += InvokeSavedScheduleEvent;
                
            UpdateView(raw, schedule);
        }
    }

    private void InvokePeriodChangedEvent(string villagerKey, int periodHour, ActivityColorData activityData)
    {
        OnPeriodChanged?.Invoke(villagerKey, periodHour, activityData);
    }

    private void InvokeSavedScheduleEvent(string scheduleKey)
    {
        OnSavedSchedule?.Invoke(scheduleKey);
    }

    private void UpdateView(ScheduleRawView scheduleRaw, Schedule schedule)
    {
        scheduleRaw.VillagerText.text = schedule.VillagerKey;
        foreach (var period in schedule.SchedulePeriods)
        {
            var periodLength = period.Length;
            for (var i = 0; i < periodLength; i++)
            {
                var time = (period.StartTime + i) % 24;
                var periodImage = scheduleRaw.PeriodImage[time];
                var color = _activitiesColorsView.ActivityColors.First(c => c.ActivityType == period.ActivityType).Color;
                periodImage.Image.color = color;
            }
        }
    }

    private void OnDestroy()
    {
        DestroySchedules();
    }

    private void DestroySchedules()
    {
        if (_schedulesRaws == null) return;
        
        foreach (var scheduleRaw in _schedulesRaws)
        {
            scheduleRaw.OnPeriodChanged -= InvokePeriodChangedEvent;
            scheduleRaw.OnSavedSchedule -= InvokeSavedScheduleEvent;
            Destroy(scheduleRaw.gameObject);
        }
        _schedulesRaws.Clear();
    }
}

