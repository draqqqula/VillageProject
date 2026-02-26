using UnityEngine;

public class SkipTimeController : MonoBehaviour
{
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private WaveStarter _waveStarter;
    
    private bool _isSkipping;
    private int _newHours;

    private void Awake()
    {
        _gameTimer.OnHourChanged += CheckNewHours;
    }
    
    public void SkipToNextWave()
    {
        Debug.Log("SkipToNextWave");
        var waveHour = _waveStarter.GetNextWaveHour();
        SkipToTime(waveHour);
    }
    
    public void SkipToTime(int newHours)
    {
        if (_isSkipping) return;
        
        Debug.Log($"StartSkipping to {newHours}");
        _isSkipping = true;
        _newHours = newHours;
        _gameTimer.IncreaseTickSpeed(20);
    }

    private void CheckNewHours(int hours)
    {
        if (!_isSkipping || hours != _newHours) return;
        StopSkipping();
    }

    public void StopSkipping()
    {
        if (!_isSkipping) return;
        
        Debug.Log($"StopSkipping in {_newHours}");
        _isSkipping = false;
        _gameTimer.ReturnTickSpeed();
    }

    private void OnDestroy()
    {
        _gameTimer.OnHourChanged -= CheckNewHours;
    }
}