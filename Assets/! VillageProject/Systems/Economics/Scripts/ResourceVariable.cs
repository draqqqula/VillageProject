using System;
using UnityEngine;

public class ResourceVariable : ScriptableObject
{
    public Action AmountChanged;
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public uint DefaultAmount { get; private set; }
    [field: NonSerialized] public uint Amount { get; private set; }
    public void Increment(uint amount)
    {
        Amount += amount;
        AmountChanged?.Invoke();
    }

    public void Decrement(uint amount)
    {
        Amount -= amount;
        AmountChanged?.Invoke();
    }

    private void Awake()
    {
        Amount = DefaultAmount;
    }
}
