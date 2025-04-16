using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DamageContext
{
    public DamageContext(IDamageComponentProvider source, IDamageComponentProvider target)
    {
        Source = source;
        Target = target;
    }

    public IDamageComponentProvider Source { get; private set; }
    public IDamageComponentProvider Target { get; private set; }
}
