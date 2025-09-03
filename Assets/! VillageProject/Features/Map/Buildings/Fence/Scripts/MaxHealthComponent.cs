using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class MaxHealthComponent : DamageComponentBase
{
    [field: SerializeField] public float MaxHealth { get; private set; }
}