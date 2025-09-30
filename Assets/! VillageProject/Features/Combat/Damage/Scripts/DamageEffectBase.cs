using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public abstract class DamageEffectBase : DamageComponentBase
{
    public abstract float Apply(DamageContext context, float baseDamage);
}
