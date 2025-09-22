using UnityEngine;

public abstract class BaseRandomizer
{
    public abstract T RandomValue<T>(T[] collection);
    public abstract T RandomValueWithProbability<T>(T[] collection) where T : IRandomizableElement;
}