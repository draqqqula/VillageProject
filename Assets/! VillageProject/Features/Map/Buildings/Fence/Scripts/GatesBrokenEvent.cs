using UnityEngine;
using UnityEngine.Events;

public class GatesBrokenEvent : MonoBehaviour
{
    public UnityEvent OnBroken;
    public UnityEvent OnFixed;

    [SerializeField] private Health _health;
    private bool _isBrokenCached = false;

    private void Start()
    {
        _health.OnDamageDealt += HandleDamageDealt;
    }

    private void HandleDamageDealt(float amount)
    {
        if (Mathf.Approximately(_health.Amount, _health.MaxHealth) && _isBrokenCached)
        {
            OnFixed.Invoke();
            _isBrokenCached = false;
        }
        else if (_health.Amount <= 0 && !_isBrokenCached)
        {
            OnBroken.Invoke();
            _isBrokenCached = true;
        }
    }
}
