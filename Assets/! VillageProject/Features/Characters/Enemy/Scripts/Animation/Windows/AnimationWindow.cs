using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation Window", menuName = "Animation Window")]
public class AnimationWindow : ScriptableObject
{
    [field: SerializeField] public string Key { get; private set; }
}