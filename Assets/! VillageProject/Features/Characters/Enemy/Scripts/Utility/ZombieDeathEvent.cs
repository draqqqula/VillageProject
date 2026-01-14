using UnityEngine;
using UnityEngine.Events;

public sealed class ZombieDeathEvent : DeathEvent
{
    [SerializeField] private BlowUp _blowUp;
    [SerializeField] private WeakSpotController _weakSpotController;
    public UnityEvent FiredBlowUp;

    protected override void OnEnable()
    {
        base.OnEnable();
        _blowUp.OnBlowedUp += BlowUpDealt;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _blowUp.OnBlowedUp -= BlowUpDealt;
    }
    
    protected override void HandleDamageDealt(float amount)
    {
        if (_health.Amount <= 0 && _weakSpotController.IsOpened.CurrentValue)
        {
            _blowUp.ActivateBlowUpImmediately();
        }
        else if (_health.Amount <= 0)
        {
            Debug.Log("Death from Healths");
            base.HandleDamageDealt(amount);
        }
    }

    private void BlowUpDealt()
    {
        Debug.Log("Death from blowup");
        _health.enabled = false;
        _blowUp.ActivateBlowUpImmediately();
        FiredBlowUp?.Invoke();
        FiredEvent?.Invoke();
    }
}