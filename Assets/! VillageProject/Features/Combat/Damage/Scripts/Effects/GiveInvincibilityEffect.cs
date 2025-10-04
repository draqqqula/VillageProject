using System;
using UnityEngine;

[Serializable]
public class GiveInvincibilityEffect : DamageEffectBase
{
    public float Factor;
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Target.TryGetService(out InvincibilityComponent invincibility)
            && context.Source.TryGetService(out KeyComponent key))
        {
            invincibility.SetKey(key.Key, Factor);
        }
        return baseDamage;
    }
}
