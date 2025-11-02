using System;
using UnityEngine;

[Serializable]
public class GuardDamageEffect : DamageEffectBase
{
    [SerializeField] private GuardConfiguration _guardConfiguration;
    [SerializeField] private Stamina _stamina;
    
    public override float Apply(DamageContext context, float baseDamage)
    {
        if (context.Source.TryGetService<BlockableDamageComponent>(out var blockableDamage)
            && _stamina.TrySpend(_guardConfiguration.StaminaWasteModifier * baseDamage))
        {
            blockableDamage.Apply();
        }
        return baseDamage;
    }
}