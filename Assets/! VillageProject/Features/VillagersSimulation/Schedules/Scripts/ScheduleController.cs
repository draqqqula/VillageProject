using System.Collections.Generic;
using System.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScheduleController : MonoBehaviour
{
    [SerializeField] private Schedule[] _schedules;
    private List<Schedule> _schedulesInstances;
    private List<Schedule> _schedulesCopies = new List<Schedule>();

    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private VillagerSystem _villagerSystem;

    [SerializeField] private ScheduleView _scheduleView;
    [Inject] private MatchObjective _matchObjective;
    [Inject] private WaveController _waveController;

    private void Awake()
    {
        CreateInstances();
        _gameTimer.OnHourChanged += OnHourChanged;
        _scheduleView.OnPeriodChanged += OnPeriodChanged;
        _scheduleView.OnSavedSchedule += OnSavedSchedule;

        _matchObjective.OnEnemiesInVillage += ActivateDefendPeriods;
        _waveController.IsOnBreak.Subscribe(OnBreakChanged).AddTo(this);
    }

    private void OnBreakChanged(bool value)
    {
        if (value) ReturnCommonPeriods();
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

    public void Init()
    {
        _scheduleView.Init(_schedulesInstances, _villagerSystem);
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

    public void UpdatePeriods(string villagerKey, ActivityType activityType, int startTime, int endTime)
    {
        var schedule = _schedulesInstances.FirstOrDefault(schedule => schedule.VillagerKey == villagerKey);
        UpdatePeriods(schedule, activityType, startTime, endTime);
    }

    private void UpdatePeriods(Schedule schedule, ActivityType activityType, int startTime, int endTime)
    {
        if (schedule == null) return;

        var length = endTime != startTime ? (endTime - startTime + 24) % 24 : 24;

        for (int i = 0; i < length; i += 1)
        {
            ChangePeriod(schedule, ConvertToHoursFormat(startTime + i), activityType);
        }

        UpdateView(schedule);
    }

    public void ActivateDefendPeriods()
    {
        if (_schedulesInstances[0].GetPeriod(0).ActivityType == ActivityType.Guard) return;
        
        foreach (var schedule in _schedulesInstances)
        {
            _schedulesCopies.Add(CopySchedules(schedule));
            UpdatePeriods(schedule, ActivityType.Guard, 20, 6);
        }
    }

    public void ReturnCommonPeriods()
    {
        foreach (var schedule in _schedulesCopies)
        {
            foreach (var period in schedule.SchedulePeriods)
            {
                UpdatePeriods(schedule.VillagerKey, period.ActivityType, period.StartTime, period.EndTime);
            }
        }
        
        _schedulesCopies.Clear();
    }
    
    private Schedule CopySchedules(Schedule schedule)
    {
        var scheduleCopy = ScriptableObject.CreateInstance<Schedule>();
        scheduleCopy.VillagerKey = schedule.VillagerKey;
        scheduleCopy.SchedulePeriods = new List<SchedulePeriod>();
        
        for (int j = 0; j < schedule.SchedulePeriods.Count; j++)
        {
            var period = schedule.SchedulePeriods[j];
            scheduleCopy.SchedulePeriods.Add(new SchedulePeriod()
            {
                StartTime = period.StartTime,
                EndTime = period.EndTime,
                ActivityType = period.ActivityType
            });
        }
        return scheduleCopy;
    }

    public Schedule GetDefaultSchedule(string villagerKey)
    {
        return _schedules.FirstOrDefault(s => s.VillagerKey == villagerKey);
    }

    public void ChangeLengthEvenlyForPeriod(string villagerKey, int hourInDefaultSchedule, int length)
    {
        var defaultSchedule = GetDefaultSchedule(villagerKey);
        var defaultPeriod = defaultSchedule.GetPeriod(hourInDefaultSchedule);

        ChangeLengthEvenlyForPeriod(villagerKey, defaultPeriod, length);
    }
    
    public void ChangeLengthEvenlyForPeriod(string villagerKey, SchedulePeriod defaultPeriod, int length)
    {
        length = Mathf.Clamp(length, 1, 24);
        
        var halfHour = defaultPeriod.Length / 2;
        var medianHour = ConvertToHoursFormat(defaultPeriod.StartTime + halfHour);
        
        int half = length / 2;
        int newStart = medianHour - half;
        int newEnd = medianHour + half;
        
        if (length % 2 == 1)
        {
            newEnd += 1;
        }
        
        ChangeStartForPeriod(villagerKey, medianHour, ConvertToHoursFormat(newStart));
        ChangeEndForPeriod(villagerKey, medianHour, ConvertToHoursFormat(newEnd));
    }
    
    public void ChangeStartForPeriod(string villagerKey, int hour, int newStartTime)
    {
        Schedule schedule;

        if (_schedulesCopies.Count == 0)
        {
            schedule = _schedulesInstances.FirstOrDefault(s => s.VillagerKey == villagerKey);
        }
        else
        {
            schedule = _schedulesCopies.FirstOrDefault(s => s.VillagerKey == villagerKey);
        }

        var period = schedule.GetPeriod(hour);
        ChangeStartForPeriod(schedule, period, newStartTime);
    }

    public void ChangeEndForPeriod(string villagerKey, int hour, int newEndTime)
    {
        Schedule schedule;

        if (_schedulesCopies.Count == 0)
        {
            schedule = _schedulesInstances.FirstOrDefault(s => s.VillagerKey == villagerKey);
        }
        else
        {
            schedule = _schedulesCopies.FirstOrDefault(s => s.VillagerKey == villagerKey);
            Debug.Log("Change Schedule Copy");
        }

        var period = schedule.GetPeriod(hour);
        ChangeEndForPeriod(schedule, period, newEndTime);
    }
    
    private void ChangeStartForPeriod(Schedule schedule, SchedulePeriod period, int newStartTime)
    {
        var activityType = period.ActivityType;
        var oldPeriodCopy = new SchedulePeriod() {ActivityType = activityType, StartTime = period.StartTime, EndTime = period.EndTime};

        UpdatePeriods(schedule, activityType, newStartTime, oldPeriodCopy.EndTime);
        var newLength = newStartTime != oldPeriodCopy.EndTime ? (oldPeriodCopy.EndTime - newStartTime + 24) % 24 : 24;
        
        if (newLength < oldPeriodCopy.Length)
        {
            ReturnToDefaultPeriods(schedule, oldPeriodCopy.StartTime, newStartTime);
            var defaultPeriod = schedule.GetPeriod(newStartTime);

            if (defaultPeriod.StartTime != newStartTime)
            {
                var previousPeriod = schedule.GetPeriod(ConvertToHoursFormat(defaultPeriod.StartTime - 1));
                UpdatePeriods(schedule, previousPeriod.ActivityType, defaultPeriod.StartTime, newStartTime);
            }
        }
    }

    private void ChangeEndForPeriod(Schedule schedule, SchedulePeriod period, int newEndTime)
    {
        var activityType = period.ActivityType;
        var oldPeriodCopy = new SchedulePeriod() {ActivityType = activityType, StartTime = period.StartTime, EndTime = period.EndTime};
        
        UpdatePeriods(schedule, activityType, oldPeriodCopy.StartTime, newEndTime);
        var newLength = oldPeriodCopy.StartTime != newEndTime ? (newEndTime - oldPeriodCopy.StartTime + 24) % 24 : 24;
        
        if (newLength < oldPeriodCopy.Length)
        {
            ReturnToDefaultPeriods(schedule, newEndTime, oldPeriodCopy.EndTime);
            var defaultPeriod = schedule.GetPeriod(newEndTime - 1);

            if (defaultPeriod.EndTime != newEndTime)
            {
                var nextPeriod = schedule.GetPeriod(ConvertToHoursFormat(defaultPeriod.EndTime));
                UpdatePeriods(schedule, nextPeriod.ActivityType, newEndTime, defaultPeriod.EndTime);
            }
        }
    }
    
    private void ReturnToDefaultPeriods(Schedule schedule, int fromPeriodInclusive, int toPeriodExclusive)
    {
        var defaultSchedule = GetDefaultSchedule(schedule.VillagerKey);
        var length = fromPeriodInclusive != toPeriodExclusive ? (toPeriodExclusive - fromPeriodInclusive + 24) % 24 : 24;

        for (int i = 0; i < length; i += 1)
        {
            var defaultActivity = defaultSchedule.GetPeriod(ConvertToHoursFormat(fromPeriodInclusive + i)).ActivityType;
            ChangePeriod(schedule, ConvertToHoursFormat(fromPeriodInclusive + i), defaultActivity);
        }
        
        UpdateView(schedule);
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

    private void UpdateView(Schedule schedule)
    {
        if (_schedulesInstances.Contains(schedule))
        {
            _scheduleView.UpdateView(schedule);
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
        _matchObjective.OnEnemiesInVillage -= ActivateDefendPeriods;
    }
}