using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public abstract class DamageConditionBase : DamageComponentBase
{
    public abstract bool IsSatisfied(DamageContext context);
}
