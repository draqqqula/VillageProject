using System;
using R3;
using UnityEngine;

public sealed class RaiseExperienceHandler : ProgressOverTimeHandler
{
    private Profession _profession;
    
    public RaiseExperienceHandler(Profession profession, GameTimer gameTimer) : base(gameTimer)
    {
        _profession = profession;
    }

    public void StartRaisingExperience()
    {
        StartRaising(_profession.ProfessionData.HoursForMaxExperience, RaiseExperience);
    }
    
    public void RaiseExperience(float progressValue)
    {
        _profession.Experience.Value = Mathf.Clamp01(_profession.Experience.Value + progressValue);
    }

    public void StopRaisingExperience()
    {
        StopRaising();
    }
}

public sealed class BuildingProgressHandler : ProgressOverTimeHandler
{
    private BuildingPlan _buildingPlan;
    public event Action OnPlanCompleted;
    
    public BuildingProgressHandler(GameTimer gameTimer) : base(gameTimer)
    {
        
    }

    public void SetPlan(BuildingPlan plan)
    {
        _buildingPlan = plan;
    }

    public void StartRaisingProgress()
    {
        StartRaising(_buildingPlan.HoursDuration, OnProgressChanged);
    }

    private void OnProgressChanged(float progressValue)
    {
        RaiseProgress(progressValue);
        if (_buildingPlan.BuildingProgress.CurrentValue >= 1)
        {
            OnPlanCompleted?.Invoke();
        }
    }
    
    public void RaiseProgress(float progressValue)
    {
        _buildingPlan.BuildingProgress.Value = Mathf.Clamp01(_buildingPlan.BuildingProgress.Value + progressValue);
    }

    public void StopRaisingProgress()
    {
        StopRaising();
    }
}

public abstract class ProgressOverTimeHandler
{
    private GameTimer _gameTimer;

    private int _lastTick = -1;
    private int _totalTicks;
    
    private Action<float> _raiseCallback;
    private bool _isRaising;
    
    public ProgressOverTimeHandler(GameTimer gameTimer)
    {
        _gameTimer = gameTimer;
    }

    protected void StartRaising(float hoursForMaxValue, Action<float> raiseProgressCallback)
    {
        if (_isRaising) return;
        
        _lastTick = -1;
        _totalTicks = _gameTimer.ConvertHoursToTick(hoursForMaxValue);
        _raiseCallback = raiseProgressCallback;
        
        _gameTimer.OnTick += RaiseValue;
        _isRaising = true;
    }

    private void RaiseValue(int currentTick)
    { 
        if (!_isRaising) return;
        
        int deltaTicks = _lastTick >= 0 ? currentTick - _lastTick : 0;
        _lastTick = currentTick;
        
        float delta = (float)deltaTicks / _totalTicks;
        _raiseCallback?.Invoke(delta);
    }
    
    protected void StopRaising()
    {
        if (!_isRaising) return;
        
        _gameTimer.OnTick -= RaiseValue;
        _isRaising = false;
    }
}