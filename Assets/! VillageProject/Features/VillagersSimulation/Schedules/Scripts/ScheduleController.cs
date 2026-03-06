using System;
using System.Collections.Generic;
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
        _scheduleView.UpdateAllView(_schedulesInstances);
    }
    
    private void OnHourChanged(int hour)
    {
        foreach (var schedule in _schedulesInstances)
        {
            foreach (var period in schedule.SchedulePeriods)
            {
                if (period.StartTime <= hour && hour < period.EndTime)
                {
                    var villager = _villagerSystem.GetVillager(schedule.VillagerKey);
                    if (villager != null) villager.ChangeActivity(period.ActivityType);
                }
            }
        }
    }

    private void OnDestroy()
    {
        _gameTimer.OnHourChanged -= OnHourChanged;
    }
}
