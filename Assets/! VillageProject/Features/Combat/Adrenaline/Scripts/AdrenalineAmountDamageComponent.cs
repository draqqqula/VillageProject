using System;
using UnityEngine;

[Serializable]
public class AdrenalineAmountDamageComponent : DamageComponentBase
{
    [field: SerializeField] public float AmountForBasicHit { get; private set; }
    [field: SerializeField] public float AmountForCriticalHit { get; private set; }
}
