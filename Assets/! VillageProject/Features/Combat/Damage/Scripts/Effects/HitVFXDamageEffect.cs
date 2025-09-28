using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HitVFXDamageEffect : DamageEffectBase
{
    [SerializeField] private GameObject _prefab;
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Source.TryGetService(out TransformDamageComponent sourceTransform)
        && context.Target.TryGetService(out TransformDamageComponent targetTransform)
        && context.Target.TryGetService(out DamageTiltableComponent tiltable))
        {
            GameObject.Instantiate(_prefab, sourceTransform.Transform.position, Quaternion.identity);
        }
        return baseDamage;
    }
}