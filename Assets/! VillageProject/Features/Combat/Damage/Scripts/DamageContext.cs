using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DamageContext
{
    public DamageContext(IServiceProvider source, IServiceProvider target, float health)
    {
        Source = source;
        Target = target;
        Health = health;
    }

    public IServiceProvider Source {  get; private set; }
    public IServiceProvider Target { get; private set; }
    public float Health { get; private set; }
}
