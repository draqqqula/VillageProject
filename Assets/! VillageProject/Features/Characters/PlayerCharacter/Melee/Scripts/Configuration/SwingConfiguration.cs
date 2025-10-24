using System.Collections;
using UnityEngine;

public class SwingConfiguration : ScriptableObject
{
    [field: SerializeField] public AnimationWindow HoldingWindow { get; private set; }
    [field: SerializeField] public AnimationCurve SlowdownCurve { get; private set; }
    [field: SerializeField] public float StaminaCost { get; private set; }
    [field: SerializeField] public float InputBufferADuration { get; private set; }
    [field: SerializeField] public float InputBufferBThreshold { get; private set; }
}