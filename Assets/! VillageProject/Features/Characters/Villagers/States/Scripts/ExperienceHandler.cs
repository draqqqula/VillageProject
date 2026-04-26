using UnityEngine;

public class ExperienceHandler
{
    private Profession _profession;
    private GameTimer _gameTimer;

    private int _lastTick = -1;
    private int _totalTicks;
    private int _curWorkTicks;

    private bool _isRaising;
    
    public ExperienceHandler(Profession profession, GameTimer gameTimer)
    {
        _profession = profession;
        _gameTimer = gameTimer;
    }

    public void StartRaisingExperience()
    {
        if (_isRaising) return;
        
        _gameTimer.OnTick += RaiseExperience;
        
        _lastTick = -1;
        _totalTicks = _gameTimer.ConvertHoursToTick(_profession.HoursForMaxExperience);
        _curWorkTicks = (int)Mathf.Floor(_gameTimer.ConvertHoursToTick(_profession.HoursForMaxExperience) * _profession.Experience.CurrentValue);
        
        _isRaising = true;
    }

    private void RaiseExperience(int currentTick)
    { 
        if (!_isRaising) return;
        
        int deltaTicks = _lastTick >= 0 ? currentTick - _lastTick : 0;
        _lastTick = currentTick;
        _curWorkTicks += deltaTicks;
        
        float progress = (float)_curWorkTicks / _totalTicks;
        _profession.Experience.Value = progress;
    }

    public void RaiseExperience(float progressValue)
    {
        _profession.Experience.Value += progressValue;
    }

    public void StopRaisingExperience()
    {
        if (!_isRaising) return;
        
        _gameTimer.OnTick -= RaiseExperience;
        _isRaising = false;
    }
}