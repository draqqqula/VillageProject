using System.Collections;
using UnityEngine;
using Zenject;

[GenerateDamageAttribute(inject: true)]
public class GainAdrenaline : IDamageExecutable
{
    [Inject] private Adrenaline _adrenaline;
    public int GetPriority()
    {
        return 0;
    }

    public bool TryExecute(DamageInteractionContext context)
    {
        if (context.TargetAttributes.TryGetService(out AdrenalineAmount amount))
        {
            _adrenaline.Gain(amount.Amount, amount.Cooldown);
        }
        return true;
    }
}