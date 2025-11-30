using UnityEngine;
using Zenject;

public class ParryingUpdater : MonoBehaviour
{
    [field: SerializeField] public bool Parrying { get; private set; }
    [SerializeField] private int _countParryingInRow;
    
    [Inject] private ParryingConfiguration _parryingConfiguration;

    private float _lastStartTime;
    private float _lastEndTime;

    public void UpdateParryingByShieldParam(float shieldValue, float maxShieldUpTime, bool isRisedShieldValue)
    {
        if (GetUnscaledLastEndTime() > _parryingConfiguration.Cooldown) _countParryingInRow = 0;
        var parryingTimingsIndex = GetParryingTimingsIndex();
        
        var parryringShieldValue = _parryingConfiguration.Timings[parryingTimingsIndex].Duration / maxShieldUpTime;
        if (parryringShieldValue > 1 || parryringShieldValue < 0) 
            Debug.LogError($"Parrying timings '{_parryingConfiguration.Timings[0].Duration}' are less then shield up time '{maxShieldUpTime}'");
        
        if (isRisedShieldValue && shieldValue > 0 &&
            shieldValue < Mathf.Clamp(parryringShieldValue, 0, 1))
        {
            UpdateParrying(true);
        }
        else UpdateParrying(false);
    }
    
    public void UpdateParrying(bool value)
    {
        if (Parrying == value) return;
        
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
        Parrying = true;
    }

    private void DeactivateParryring()
    {
        _lastEndTime = Time.unscaledTime;
        Parrying = false;
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
}