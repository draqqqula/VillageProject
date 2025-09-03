using System;
using UnityEngine;

[Serializable]
public class StunDamageEffect : DamageEffectBase
{
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Target.TryGetComponent<StunDamageComponent>(out var stun))
        {
            stun.Apply(baseDamage);
        }
        return baseDamage;
    }
}
