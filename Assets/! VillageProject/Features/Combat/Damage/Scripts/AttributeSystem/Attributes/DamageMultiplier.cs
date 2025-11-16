using System.Collections;
using UnityEngine;

[GenerateDamageAttribute]
public class DamageMultiplier : IDamageExecutable
{
    [FromDamageAttributeProperty] public float Multiplier;
    public bool TryExecute(DamageInteractionContext context)
    {
        if (context.SourceAttributes.TryGetService(out BaseDamageAmountAttribute.Effect baseDamage))
        {
            baseDamage.Amount *= Multiplier;
        }
        return true;
    }
}