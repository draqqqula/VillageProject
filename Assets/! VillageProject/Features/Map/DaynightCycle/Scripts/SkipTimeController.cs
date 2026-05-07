using System;
using R3;
using UnityEngine;
using Zenject;

public class SkipTimeController : MonoBehaviour
{
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private WaveStarter _waveStarter;
    
    public ReadOnlyReactiveProperty<bool> IsSkipping => _isSkipping;
    private ReactiveProperty<bool> _isSkipping = new ReactiveProperty<bool>(false);
    
    private int _tickToSkip;

    private void Awake()
    {
        _gameTimer.OnTick += CheckNewTick;
    }
    
    public void SkipToNextWave()
    {
        var waveHour = _waveStarter.GetNextWaveHour();
        SkipToTime(waveHour - 1, 30);
    }
    
    public void SkipToTime(int newHours, int newMinutes)
    {
        if (IsSkipping.CurrentValue) return;
        
        Debug.Log($"StartSkipping to {newHours}:{newMinutes}");
        _isSkipping.Value = true;

        var ticksBetween = GetTicksBetween(_gameTimer.CurrentHour, _gameTimer.CurrentMinute, newHours, newMinutes);
        _tickToSkip = _gameTimer.CurrentTick + ticksBetween;
        
        _gameTimer.IncreaseTickSpeed(20);
    }

    private int GetTicksBetween(int startHours, int startMinutes, int stopHours, int stopMinutes)
    {
        var currentMinutes = startHours * 60 + startMinutes;
        var targetMinutes = stopHours * 60 + stopMinutes;
        var skipMinutes = (targetMinutes - currentMinutes + 60 * 24) % (60 * 24);
        return _gameTimer.ConvertMinutesToTick(skipMinutes);
    }

    private void CheckNewTick(int tick)
    {
        if (!IsSkipping.CurrentValue || tick < _tickToSkip) return;
        StopSkipping();
    }

    public void StopSkipping()
    {
        if (!IsSkipping.CurrentValue) return;
        
        Debug.Log($"StopSkipping in {_gameTimer.CurrentHour}:{_gameTimer.CurrentMinute}");
        _isSkipping.Value = false;
        _gameTimer.ReturnTickSpeed();
    }

    private void OnDestroy()
    {
        _gameTimer.OnTick -= CheckNewTick;
    }
}