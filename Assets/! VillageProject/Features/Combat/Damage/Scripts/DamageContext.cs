using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DamageContext
{
    public DamageContext(IDamageComponentProvider source, IDamageComponentProvider target, float health)
    {
        Source = source;
        Target = target;
        Health = health;
    }

    public IDamageComponentProvider Source { get; private set; }
    public IDamageComponentProvider Target { get; private set; }
    public float Health { get; private set; }
}
