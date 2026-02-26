using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private const int HoursPerDay = 24;

    [SerializeField] private int _ticksPerHour;
    [SerializeField] private float _realSecondsPerTick = 0.1f;
    private float _startRealSecondsPerTick;

    [SerializeField] private int _startHour;
    [SerializeField] private int _hoursOffset;

    private int _startTicks;
    private int _ticksOffset;
    private int _realTicks;

    private float _timer;
    private int _currentTick;

    public int CurrentTick => _currentTick;
    public int CurrentDay { get; private set; }
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }

    [SerializeField] private CycleFromTime _cycleFromTime;
    [SerializeField] private TimeView _timeView;

    public event Action<int> OnHourChanged;
    public event Action<int> OnDayChanged;
    
    private bool _isPaused;

    private void Awake()
    {
        _startTicks = _startHour * _ticksPerHour;
        _ticksOffset = _hoursOffset * _ticksPerHour;
        _currentTick  = _startTicks;
        _startRealSecondsPerTick = _realSecondsPerTick;
    }
    
    private void Update()
    {
        if (_isPaused) return;
        
        _timer += Time.deltaTime;

        while (_timer >= _realSecondsPerTick)
        {
            _timer -= _realSecondsPerTick;
            AddTick();
        }
        
        _timeView.UpdateTime();
    }
    
    public void AddTick()
    {
        _realTicks++;
        _currentTick++;

        int totalHours = _currentTick / _ticksPerHour;
        int newDay = totalHours / HoursPerDay;
        int ticksPerDay = HoursPerDay * _ticksPerHour;
        
        float dayProgress = (float)(_currentTick - _ticksOffset) % ticksPerDay / ticksPerDay;
        _cycleFromTime.SetTime(dayProgress);
        
        int newHour = totalHours % HoursPerDay;

        int ticksIntoHour = _currentTick % _ticksPerHour;
        int newMinute = (int)((float)ticksIntoHour / _ticksPerHour * 60f);

        if (newHour != CurrentHour)
        {
            CurrentHour = newHour;
            OnHourChanged?.Invoke(CurrentHour);
        }

        if (newDay != CurrentDay)
        {
            CurrentDay = newDay;
            OnDayChanged?.Invoke(CurrentDay);
        }

        CurrentMinute = newMinute;
    }
    
    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
    }
    
    public void IncreaseTickSpeed(float multiplier)
    {
        _realSecondsPerTick *= (1/multiplier);
    }

    public void DecreaseTickSpeed(float multiplier)
    {
        _realSecondsPerTick *= multiplier;
    }

    public void ReturnTickSpeed()
    {
        _realSecondsPerTick = _startRealSecondsPerTick;
    }
}