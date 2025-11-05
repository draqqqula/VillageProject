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
        using var targetScope = target.CreateScope();
        using var sourceScope = source.CreateScope();
        var effects = targetScope.ServiceProvider.GetServices<IDamageExecutable>().Concat(sourceScope.ServiceProvider.GetServices<IDamageExecutable>());

        var context = new DamageInteractionContext(targetScope.ServiceProvider, sourceScope.ServiceProvider);

        foreach (var effect in effects)
        {
            if (!effect.TryExecute(context))
            {
                break;
            }
        }
    }

    public static void Interact(this IDamageInteractable target, IDamageInteractable source)
    {
        Interact(target.AttributeProvider, source.AttributeProvider);
    }
}
