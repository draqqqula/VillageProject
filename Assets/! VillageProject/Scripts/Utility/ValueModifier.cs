using System;
using System.Collections;
using UnityEngine;

public abstract class ValueModifier<T>
{
    public abstract T Apply(T value);
}