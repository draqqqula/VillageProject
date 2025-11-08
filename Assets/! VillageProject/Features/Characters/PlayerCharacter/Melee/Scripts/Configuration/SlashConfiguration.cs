using System.Collections;
using UnityEngine;

public class SlashConfiguration : ScriptableObject
{
    [field: SerializeField] public AnimationWindow SlashWindow { get; private set; }
    [field: SerializeField] public AnimationWindow ThrustWindow { get; private set; }
}