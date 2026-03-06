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
    
    public void UpdateAllView(List<Schedule> schedules)
    {
        if (_schedulesRaws != null) DestroySchedules();
        
        foreach (var schedule in schedules)
        {
            var raw = Instantiate(_scheduleRawPrefab, _schedulesParent);
            UpdateView(raw, schedule);
        }
    }

    private void UpdateView(ScheduleRawView scheduleRaw, Schedule schedule)
    {
        scheduleRaw.VillagerText.text = schedule.VillagerKey;
        foreach (var period in schedule.SchedulePeriods)
        {
            int endTime = period.EndTime;
            int startTime = period.StartTime;

            var periodLength = endTime - startTime;
            if (endTime < startTime) periodLength = 24 - startTime + endTime;
            
            for (var i = 0; i < periodLength; i++)
            {
                var time = (startTime + i) % 24;
                var periodImage = scheduleRaw.PeriodImage[time];
                var color = _activitiesColorsView.ActivityColors.First(c => c.ActivityType == period.ActivityType).Color;
                periodImage.color = color;
            }
        }
    }

    private void DestroySchedules()
    {
        foreach (var scheduleRaw in _schedulesRaws)
        {
            Destroy(scheduleRaw.gameObject);
        }
        _schedulesRaws.Clear();
    }
}

