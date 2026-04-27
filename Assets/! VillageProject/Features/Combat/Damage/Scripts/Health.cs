using Microsoft.Extensions.DependencyInjection;
using R3;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] private float _maxHealth;
    private ReactiveProperty<float> _amount = new ReactiveProperty<float>();
    private ReactiveProperty<float> _maxAmountReactive;
    
    public float DefaultMaxHealth { get; private set; }

    public event Action<float> OnDamageDealt;

    public float Amount
    {
        get
        {
            return _amount.Value;
        }
        set
        {
            var cached = _amount.Value;
            _amount.Value = value;
            OnDamageDealt?.Invoke(cached - _amount.Value);
        }
    }

    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxAmountReactive.Value = value;
        }
    }
    
    public IServiceProvider ComponentProvider { get; private set; }
    public ReadOnlyReactiveProperty<float> AmountReactive => _amount;
    public ReadOnlyReactiveProperty<float> MaxAmountReactive => _maxAmountReactive;

    [ContextMenu("Print")]
    public void Print()
    {
        Debug.Log(_amount.Value);
    }

    private void Awake()
    {
        _amount.Value = MaxHealth;
        DefaultMaxHealth = _maxHealth;
        
        _maxAmountReactive = new ReactiveProperty<float>(_maxHealth);
        _maxAmountReactive.Subscribe(it => _maxHealth = it).AddTo(this);
    }
}
