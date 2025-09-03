using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class BurningDamageEffect : DamageEffectBase
{
    [SerializeField] private GameObject _vfx;
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Target.TryGetComponent<TransformDamageComponent>(out var transform))
        {
            GameObject.Instantiate(_vfx, transform.Transform);
        }
        return baseDamage;
    }
}