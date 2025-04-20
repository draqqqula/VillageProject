using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class KnockbackDamageEffect : DamageEffectBase
{
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Source.TryGetComponent(out TransformDamageComponent sourceTransform)
            && context.Target.TryGetComponent(out TransformDamageComponent targetTransform)
            && context.Target.TryGetComponent(out DamageTiltableComponent tiltable))
        {
            tiltable.DamageTilt.Create((sourceTransform.Transform.position - targetTransform.Transform.position).ToXZ().ToVector3XZ());
        }
        return baseDamage;
    }
}
