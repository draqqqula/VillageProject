using System;
using UnityEngine;

public sealed class RaiseArrowsHandler : ResourceOverTimeHandler
{
    private int _hoursForRaisingAmmunition;
    private int _increasedHour;
    
    private TowerAmmunition _ammunition;
    private AmmunitionStorage _ammunitionStorage;
    
    public TowerAmmunition Ammunition => _ammunition;
    public AmmunitionStorage AmmunitionStorage => _ammunitionStorage;
    
    public RaiseArrowsHandler(TowerAmmunition ammunition, AmmunitionStorage ammunitionStorage, GameTimer gameTimer) : base(gameTimer)
    {
        _ammunition = ammunition;
        _ammunitionStorage = ammunitionStorage;
    }

    public void IncreaseRaiseArrows(int hour)
    {
        _increasedHour = hour;
    }

    public void StartRaisingArrows(float hoursForRaisingAmmunition)
    {
        StartRaising(hoursForRaisingAmmunition + _increasedHour, _ammunition.MaxAmount, OnChangedAmount);
    }

    public void RaiseArrow(uint amount)
    {
        OnChangedAmount(_ammunitionStorage.Amount.CurrentValue.Amount + amount);
    }
    
    private void OnChangedAmount(uint value)
    {
        if (value > 0)
        {
            if (_ammunitionStorage.CanStore(_ammunition, value))
            {
                _ammunitionStorage.TryStore(_ammunition, value);
            }
        }
    }

    public void StopRaisingArrows()
    {
        StopRaising();
        _increasedHour = 0;
    }
}

public abstract class ResourceOverTimeHandler
{
    private GameTimer _gameTimer;

    private int _lastTick = -1;
    private int _totalTicks;
    
    private Action<uint> _riseCallback;

    private uint _maxAmount;
    private float _fractionalPart;

    private bool _isRaising;
    
    public ResourceOverTimeHandler(GameTimer gameTimer)
    {
        _gameTimer = gameTimer;
    }

    public void StartRaising(float hoursForMaxValue, uint maxAmount, Action<uint> riseAmountCallback)
    {
        if (_isRaising) return;
        
        _lastTick = -1;
        _totalTicks = _gameTimer.ConvertHoursToTick(hoursForMaxValue);
        _maxAmount = maxAmount;
        
        _riseCallback = riseAmountCallback;
        
        _gameTimer.OnTick += RaiseValue;
        _isRaising = true;
    }

    private void RaiseValue(int currentTick)
    { 
        if (!_isRaising) return;
        
        int deltaTicks = _lastTick >= 0 ? currentTick - _lastTick : 0;
        _lastTick = currentTick;
        
        float delta = ((float)deltaTicks / _totalTicks) * _maxAmount;
        _fractionalPart += delta;
        
        var wholePart = (uint)Mathf.Floor(_fractionalPart);
        _fractionalPart -= wholePart;
        
        _riseCallback?.Invoke(wholePart);
    }
    
    protected void StopRaising()
    {
        if (!_isRaising) return;
        
        _gameTimer.OnTick -= RaiseValue;
        _isRaising = false;
    }
}