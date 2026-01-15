using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using UnityEngine;

[GenerateDamageAttribute]
public class OnlyDirectionCondition : IDamageExecutable
{
    [FromDamageAttributeProperty] public AttackDirection Required;
    public int GetPriority()
    {
        return 0;
    }

    public bool TryExecute(DamageInteractionContext context)
    {
        if (context.SourceAttributes.TryGetService(out AttackDirectionReader reader))
        {
            if (reader.GetDirection() == Required)
            {
                return true;
            }
            return false;
        }
        return true;
    }
}