using R3;
using System;
using System.Collections;
using System.Collections.Generic;

public class ModifiableValue<T>
{
    class ModifierComparer : IComparer<ModifierAndPriority>
    {
        public int Compare(ModifierAndPriority x, ModifierAndPriority y)
        {
            return x.Priority - y.Priority;
        }
    }

    class ModifierAndPriority
    {
        public readonly ValueModifier<T> Modifier;
        public readonly int Priority;

        public ModifierAndPriority(ValueModifier<T> modifier, int priority)
        {
            Modifier = modifier;
            Priority = priority;
        }
    }

    private T _baseValue;
    private ReactiveProperty<T> _value;
    private SortedSet<ModifierAndPriority> _modifiers;

    public ModifiableValue(T baseValue)
    {
        _baseValue = baseValue;
        _value = new ReactiveProperty<T>(baseValue);
        _modifiers = new SortedSet<ModifierAndPriority>(new ModifierComparer());
    }

    public T BaseValue
    {
        get
        {
            return _baseValue;
        }
        set
        {
            _baseValue = value;
            UpdateValue();
        }
    }

    public ReadOnlyReactiveProperty<T> Value => _value;

    public IDisposable AddModifier(ValueModifier<T> modifier, int priority)
    {
        var pair = new ModifierAndPriority(modifier, priority);
        _modifiers.Add(pair);
        UpdateValue();
        return Disposable.Create(() =>
        {
            _modifiers.Remove(pair);
            UpdateValue();
        });
    }

    public void UpdateValue()
    {
        _value.Value = CalculateValue();
    }

    private T CalculateValue()
    {
        var value = _baseValue;
        foreach (var pair in _modifiers)
        {
            value = pair.Modifier.Apply(value);
        }
        return value;
    }
}