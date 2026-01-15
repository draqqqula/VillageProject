using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class TeamConditionAttribute : DamageAttributeBase
{
    public class Condition : IDamageExecutable
    {
        public int GetPriority()
        {
            return 0;
        }

        public bool TryExecute(DamageInteractionContext context)
        {
            if (context.TargetAttributes.TryGetService<ITeamAttribute>(out var targetTeam)
                && context.SourceAttributes.TryGetService<ITeamAttribute>(out var sourceTeam)
                && !targetTeam.Team.IsEnemiesWith(sourceTeam.Team))
            {
                return false;
            }
            return true;
        }
    }

    public override void AddServices(IServiceCollection services)
    {
        services.AddSingleton<Condition>();
        services.AddSingleton<IDamageExecutable>(provider => provider.GetService<Condition>());
    }
}