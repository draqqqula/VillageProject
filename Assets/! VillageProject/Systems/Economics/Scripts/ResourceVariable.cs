using R3;
using System;
using UnityEngine;

public class ResourceVariable : ScriptableObject
{
    public Action AmountChanged;
    private ReactiveProperty<uint> _amount;
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public uint DefaultAmount { get; private set; }
    public uint Amount => _amount?.CurrentValue ?? DefaultAmount;
    public ReadOnlyReactiveProperty<uint> AmountReactive => _amount;

    public void Increment(uint amount)
    {
        _amount.Value += amount;
        AmountChanged?.Invoke();
    }

    public void Decrement(uint amount)
    {
        _amount.Value -= amount;
        AmountChanged?.Invoke();
    }

    private void OnEnable()
    {
        _amount = new ReactiveProperty<uint>(DefaultAmount);
    }
}
