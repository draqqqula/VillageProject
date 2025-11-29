using System;
using UnityEngine;

public class GuardConfiguration : ScriptableObject
{
    [field: SerializeField] public AnimationWindow Window { get; private set; }
    [field: SerializeField] public float StaminaFillModifier { get; private set; }
    [field: SerializeField] public float StaminaWasteModifier { get; private set; }
    [field: SerializeField] public float MaxHoldingTime { get; private set; }
    [field: SerializeField] public AnimationCurve SlowdownCurve { get; private set; }
}