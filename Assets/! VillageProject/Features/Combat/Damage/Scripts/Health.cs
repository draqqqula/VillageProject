using Microsoft.Extensions.DependencyInjection;
using R3;
using System;
using UnityEngine;

public class Health : MonoBehaviour, IHealth
{
    private ReactiveProperty<float> _amount = new ReactiveProperty<float>();

    public event Action<float> OnDamageDealt;

    public float Amount
    {
        get
        {
            return _amount.Value;
        }
        set
        {
            _amount.Value = value;
            OnDamageDealt?.Invoke(_amount.Value - value);
        }
    }

    [field: SerializeField] public float MaxAmount { get; private set; }
    
    public IServiceProvider ComponentProvider { get; private set; }
    public ReadOnlyReactiveProperty<float> AmountReactive => _amount;

    private void Awake()
    {
        _amount.Value = MaxAmount;
    }
}
