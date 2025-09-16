using System;
using UnityEngine;

[Serializable]
public class GainAdrenalineDamageEffect : DamageEffectBase
{
    [SerializeField] private Adrenaline _adrenaline;
    public override float Apply(DamageContext context, float baseDamage)
    {
        _adrenaline.Gain(baseDamage, 1);
        return baseDamage;
    }
}
