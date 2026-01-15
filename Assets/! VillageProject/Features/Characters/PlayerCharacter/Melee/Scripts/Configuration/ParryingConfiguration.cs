using System;
using UnityEngine;

public class ParryingConfiguration : ScriptableObject
{
    [field: SerializeField] public ParryingTimings[] Timings { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
}

[Serializable]
public class ParryingTimings
{
    [field: SerializeField] public float Duration { get; private set; }
}