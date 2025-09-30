using System;
using UnityEngine;

[Serializable]
public class StunDamageEffect : DamageEffectBase
{
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Target.TryGetService<StunDamageComponent>(out var stun))
        {
            stun.Apply(baseDamage);
        }
        return baseDamage;
    }
}
