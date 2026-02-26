using System.Linq;
using R3;
using UnityEngine;

public class WaveStarter : MonoBehaviour
{
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private int[] _startWaveHours;
    [SerializeField] private WaveController _waveController;

    private int _maxHoursInWave = 2;
    private int _waveHour;

    private void Awake()
    {
        _gameTimer.OnHourChanged += OnHourChanged;
    }

    private void OnHourChanged(int hour)
    {
        if (_waveController.IsOnBreak.CurrentValue)
        {
            if (_startWaveHours.Any(h => hour == h)) StartWave();
        }
        else
        {
            if (hour >= _waveHour + _maxHoursInWave) _gameTimer.Pause();
        }
    }

    private void StartWave()
    {
        _waveHour = _gameTimer.CurrentHour;
        _gameTimer.DecreaseTickSpeed(10);
        
        _waveController.FinishBreak();
        _waveController.IsOnBreak.Subscribe(OnBreakChanged).AddTo(this);
    }

    private void OnBreakChanged(bool isOnBreak)
    {
        if (isOnBreak) FinishWave();
    }

    private void FinishWave()
    {
        _gameTimer.ReturnTickSpeed();
        _gameTimer.Resume(); 
    }
    
    public int GetNextWaveHour()
    {
        var earliestWaveHourInTheDay = 24;
        var closestWaveHour = 40;
        foreach (var waveHour in _startWaveHours)
        {
            if (waveHour > _gameTimer.CurrentHour && waveHour < closestWaveHour) closestWaveHour = waveHour;
            if (waveHour < earliestWaveHourInTheDay) earliestWaveHourInTheDay = waveHour;
        }
        
        if (closestWaveHour <= 24 && closestWaveHour > _gameTimer.CurrentHour) return closestWaveHour;
        
        Debug.Log("Waves started in the next day!");
        return earliestWaveHourInTheDay;
    }
}