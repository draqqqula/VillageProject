using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScheduleView : MonoBehaviour
{
    [SerializeField] private string _inputActionMapName;
    [SerializeField] private InputActionAsset _asset;
    
    [SerializeField] private ActivityColorView _activitiesColorsView;
    
    [SerializeField] private GameTimer _gameTimer;

    [SerializeField] private ScheduleRawView _scheduleRawPrefab;
    [SerializeField] private Transform _schedulesParent;
    [SerializeField] private Transform _contentParent;
    [SerializeField] private Slider _timeSlider;
    
    private List<ScheduleRawView> _schedulesRaws = new List<ScheduleRawView>();

    public event Action<string> OnSavedSchedule;
    public event Action<string, int, ActivityColorData> OnPeriodChanged;
    public event Action<string, ProfessionType> OnProfessionChanged;

    public void Init(List<Schedule> schedules, VillagerSystem villagerSystem)
    {
        UpdateTimeView(_gameTimer.CurrentHour);
        _gameTimer.OnHourChanged += UpdateTimeView;
        
        UpdateAllSchedulesView(schedules, villagerSystem);
    }
    
    private void OnEnable()
    {
        _asset.FindActionMap(_inputActionMapName).Disable();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        _asset.FindActionMap(_inputActionMapName).Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void UpdateAllSchedulesView(List<Schedule> schedules, VillagerSystem villagerSystem)
    {
        if (_schedulesRaws != null && _schedulesRaws.Count > 0) DestroySchedules();
        
        foreach (var schedule in schedules)
        {
            var raw = Instantiate(_scheduleRawPrefab, _schedulesParent);
            raw.Init(_activitiesColorsView);
            raw.OnPeriodChanged += InvokePeriodChangedEvent;
            raw.OnSavedSchedule += InvokeSavedScheduleEvent;
            raw.CurrentProfession.Subscribe((profession) => InvokeProfessionChanged(schedule.VillagerKey, profession))
                .AddTo(this);
         
            UpdateView(raw, schedule);
            
            var villager = villagerSystem.GetVillager(schedule.VillagerKey);
            raw.ProfessionMenu.Init(villager.VillagerData.Profession.Type, _contentParent);

            _schedulesRaws.Add(raw);
        }
    }

    public void UpdateView(Schedule schedule)
    {
        var raw = _schedulesRaws.FirstOrDefault(x => x.VillagerText.text == schedule.VillagerKey);
        UpdateView(raw, schedule);
    }

    private void InvokePeriodChangedEvent(string villagerKey, int periodHour, ActivityColorData activityData)
    {
        OnPeriodChanged?.Invoke(villagerKey, periodHour, activityData);
    }

    private void InvokeSavedScheduleEvent(string scheduleKey)
    {
        OnSavedSchedule?.Invoke(scheduleKey);
    }

    private void InvokeProfessionChanged(string villagerKey, ProfessionType profession)
    {
        Debug.Log("Invoke changing profession in shedule view!");
        OnProfessionChanged?.Invoke(villagerKey, profession);
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

    private void UpdateTimeView(int hour)
    {
        _timeSlider.value = hour;
    }
    
    public void UpdateProfessionView(string villager, ProfessionType professionType)
    {
        var schedule = _schedulesRaws.FirstOrDefault(s => s.VillagerText.text == villager);
        schedule?.ProfessionMenu.SetProfession(professionType);
    }

    private void OnDestroy()
    {
        _gameTimer.OnHourChanged -= UpdateTimeView;
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