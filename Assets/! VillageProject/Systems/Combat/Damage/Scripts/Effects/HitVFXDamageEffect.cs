using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class HitVFXDamageEffect : DamageEffectBase
{
    [SerializeField] private GameObject _prefab;
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Source.TryGetComponent(out TransformDamageComponent sourceTransform)
        && context.Target.TryGetComponent(out TransformDamageComponent targetTransform)
        && context.Target.TryGetComponent(out DamageTiltableComponent tiltable))
        {
            GameObject.Instantiate(_prefab, sourceTransform.Transform.position, Quaternion.identity);
        }
        return baseDamage;
    }
}