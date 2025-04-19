using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Damage", 
    menuName = "Damage Info", 
    order = 99)]
public class DamageInfo : ScriptableObject
{
    [field: SerializeField] public DamageData Data { get; private set; }
    [field: SerializeField] public float BaseAmount { get; private set; }
}
