using System;
using System.Collections;
using UnityEngine;

public class FloatMultiplierModifier : ValueModifier<float>
{
    public float Multiplier;
    public FloatMultiplierModifier(float multiplier)
    {
        Multiplier = multiplier;
    }

    public override float Apply(float value)
    {
        return Multiplier * value;
    }
}

public static class FloatModifierExtensions
{
    public static IDisposable AddMultiplier(this ModifiableValue<float> modifiable, float multiplier)
    {
        return modifiable.AddModifier(new FloatMultiplierModifier(multiplier), 0);
    }
}
