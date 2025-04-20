using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class EnemyTeamCondition : DamageConditionBase
{
    public override bool IsSatisfied(DamageContext context)
    {
        return context.Source.TryGetComponent<TeamDamageComponent>(out var sourceTeam)
            && context.Target.TryGetComponent<TeamDamageComponent>(out var targetTeam)
            && sourceTeam.Member.Team.IsEnemiesWith(targetTeam.Member.Team);
    }
}
