using System.Linq;
using R3;
using UnityEngine;

public class WaveStarter : MonoBehaviour
{
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private int[] _startWaveHours;
    [SerializeField] private WaveController _waveController;

    private int _maxHoursInWave = 2;
    private int _pauseTick;

    [SerializeField] private int _startAllWavesHour = 20;
    [SerializeField] private int _endAllWavesHour = 5;
    
    private void Awake()
    {
        _gameTimer.OnHourChanged += OnHourChanged;
    }

    private void OnHourChanged(int hour)
    {
        if (_waveController.IsOnBreak.CurrentValue)
        {
            if (_startWaveHours.Any(h => hour == h))
            {
                if (_startWaveHours[_startWaveHours.Length - 1] == hour) _waveController.IsWaveInNextNight = true;
                else _waveController.IsWaveInNextNight = false;
                
                StartWave();
            }
        }
        else
        {
            if (_gameTimer.CurrentTick >= _pauseTick)
            {
                _gameTimer.Pause();
            }
        }
    }

    private void StartWave()
    {
        _pauseTick = _gameTimer.CurrentTick + _gameTimer.ConvertHoursToTick(_maxHoursInWave);
        var startTick = _gameTimer.CurrentTick - _gameTimer.ConvertHoursToTick((_gameTimer.CurrentHour - _startAllWavesHour + 24) % 24);
        var endTick   = startTick + _gameTimer.ConvertHoursToTick((_endAllWavesHour - _startAllWavesHour + 24) % 24);
        _pauseTick = Mathf.Clamp(_pauseTick, startTick, endTick);
        
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