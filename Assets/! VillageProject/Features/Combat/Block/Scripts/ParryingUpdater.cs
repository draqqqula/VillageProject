using R3;
using UnityEngine;
using Zenject;

public class ParryingUpdater : MonoBehaviour
{
    [field: SerializeField] public ReadOnlyReactiveProperty<bool> Parrying => _parrying;
    private ReactiveProperty<bool> _parrying = new ReactiveProperty<bool>(false);
    
    [SerializeField] private int _countParryingInRow;
    
    [Inject] private ParryingConfiguration _parryingConfiguration;

    private float _lastStartTime;
    private float _lastEndTime;

    private float _currentShieldValue;
    private float _maxParryingShieldValue;

    public void UpdateParryingByShieldParam(float shieldValue, float maxShieldUpTime, bool isRisedShieldValue)
    {
        if (GetUnscaledLastEndTime() > _parryingConfiguration.Cooldown) _countParryingInRow = 0;
        
        _currentShieldValue = shieldValue;
        var parryingTimingsIndex = GetParryingTimingsIndex();
        
        _maxParryingShieldValue = _parryingConfiguration.Timings[parryingTimingsIndex].Duration / maxShieldUpTime;
        if (_maxParryingShieldValue > 1 || _maxParryingShieldValue < 0) 
            Debug.LogError($"Parrying timings '{_parryingConfiguration.Timings[0].Duration}' are less then shield up time '{maxShieldUpTime}'");
        
        if (isRisedShieldValue && shieldValue > 0 &&
            shieldValue < Mathf.Clamp(_maxParryingShieldValue, 0, 1))
        {
            UpdateParrying(true);
        }
        else UpdateParrying(false);
    }
    
    public void UpdateParrying(bool value)
    {
        if (Parrying.CurrentValue == value) return;
        
        if (value)
        {
            if (GetUnscaledLastEndTime() <= _parryingConfiguration.Cooldown) _countParryingInRow++;
            ActivateParryring();
        }
        else DeactivateParryring();
    }

    private void ActivateParryring()
    {
        _lastStartTime = Time.unscaledTime;
        _parrying.Value = true;
    }

    private void DeactivateParryring()
    {
        _lastEndTime = Time.unscaledTime;
        _parrying.Value = false;
    }
    
    private float GetUnscaledLastEndTime()
    {
        return Time.unscaledTime - _lastEndTime;
    }

    private float GetUnscaledLastStartTime()
    {
        return Time.unscaledTime - _lastStartTime;
    }

    private int GetParryingTimingsIndex()
    {
        return Mathf.Clamp(_countParryingInRow, 0, _parryingConfiguration.Timings.Length - 1);
    }

    public float GetCurrentParryingTime()
    {
        if (!Parrying.CurrentValue) return 0;
        return _currentShieldValue / _maxParryingShieldValue;
    }
}