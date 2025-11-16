using System.Collections;
using UnityEngine;
using Zenject;

[GenerateDamageAttribute(inject: true)]
public class GainAdrenaline : IDamageExecutable
{
    [Inject] private Adrenaline _adrenaline;

    public bool TryExecute(DamageInteractionContext context)
    {
        if (context.TargetAttributes.TryGetService(out AdrenalineAmount amount))
        {
            _adrenaline.Gain(amount.Amount, amount.Cooldown);
        }
        return true;
    }
}