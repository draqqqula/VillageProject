using System;
using UnityEngine;

[Serializable]
public class GainAdrenalineDamageEffect : DamageEffectBase
{
    [SerializeField] private Adrenaline _adrenaline;
    [SerializeField] private float _cooldown;
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Target.TryGetService(out AdrenalineAmountDamageComponent amount))
        {
            if (context.Source.TryGetService(out HitWeakSpotEffect.CriticalHit criticalHit)
                && criticalHit.Flag)
            {
                Debug.Log("critical");
                _adrenaline.Gain(amount.AmountForCriticalHit, _cooldown);
            }
            else
            {
                Debug.Log("base");
                _adrenaline.Gain(amount.AmountForBasicHit, _cooldown);
            }
        }
        return baseDamage;
    }
}
