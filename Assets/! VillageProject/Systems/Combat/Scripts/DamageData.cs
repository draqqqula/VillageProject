using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DamageData
{
    [SerializeReference, SubclassSelector] public List<DamageComponentBase> Components;
    [SerializeReference, SubclassSelector] public List<DamageConditionBase> Conditions;
    [SerializeReference, SubclassSelector] public List<DamageEffectBase> Effects;
}