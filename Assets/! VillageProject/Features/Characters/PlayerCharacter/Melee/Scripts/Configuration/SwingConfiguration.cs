using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwingConfiguration : ScriptableObject
{
    [field: SerializeField] public AnimationWindow HoldingWindow { get; private set; }
    [field: SerializeField] public AnimationCurve SlowdownCurve { get; private set; }
    [field: SerializeField] public float StaminaCost { get; private set; }
    [field: SerializeField] public float InputBufferADuration { get; private set; }
    [field: SerializeField] public float InputBufferBThreshold { get; private set; }
    [field: SerializeField] public float BlendingSpeed { get; private set; }
    [field: SerializeField] public float ThrustBorder { get; private set; }
    [field: SerializeField] public float MaxDeltaMagnitude { get; private set; }
    [field: SerializeField] public float WeightToBlendRatio { get; private set; }
    [field: SerializeField] public float ShiftSensitivityMultiplier { get; set; }
    [field: SerializeField] public float CursorSmoothTime { get; set; }
    [field: SerializeField] public float CursorSensitivity { get; set; }
    [field: SerializeField] public float SwitchSoundCooldown { get; private set; }
    [field: SerializeField] public InputActionReference ShiftInput { get; private set; }
    [field: SerializeField] public InputActionReference LookInput { get; private set; }
}