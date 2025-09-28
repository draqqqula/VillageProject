using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public abstract class DamageComponentBase
{
    public virtual void RegisterTo(IServiceCollection services)
    {
        services.AddSingleton(this);
        services.AddSingleton(GetType(), this);
    }
}
