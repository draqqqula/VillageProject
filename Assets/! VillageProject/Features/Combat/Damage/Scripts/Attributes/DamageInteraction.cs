using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class DamageInteraction
{
    public static void Interact(IServiceProvider target, IServiceProvider source)
    {
        var effects = target.GetServices<IDamageEffect>().Concat(source.GetServices<IDamageEffect>());
        var context = new DamageInteractionContext(target, source);
        foreach (var effect in effects)
        {
        }
    }

    public static void Interact(this IDamageAttributeProvider target, IDamageAttributeProvider source)
    {
        Interact(target.Provider, source.Provider);
    }
}
