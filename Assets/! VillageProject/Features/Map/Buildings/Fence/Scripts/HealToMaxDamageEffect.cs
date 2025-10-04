using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HealToMaxDamageEffect : DamageEffectBase
{
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Target.TryGetService<MaxHealthComponent>(out var maxHealth))
        {
            return context.Health - maxHealth.MaxHealth;
        }
        return baseDamage;
    }
}