using System;
using System.Collections;
using UnityEngine;

public class FloatMultiplierModifier : ValueModifier<float>
{
    private float _multiplier;
    public FloatMultiplierModifier(float multiplier)
    {
        _multiplier = multiplier;
    }

    public override float Apply(float value)
    {
        return _multiplier * value;
    }
}

public static class FloatModifierExtensions
{
    public static IDisposable AddMultiplier(this ModifiableValue<float> modifiable, float multiplier)
    {
        return modifiable.AddModifier(new FloatMultiplierModifier(multiplier), 0);
    }
}
