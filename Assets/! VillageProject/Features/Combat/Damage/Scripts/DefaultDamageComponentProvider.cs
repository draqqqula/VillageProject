using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DefaultDamageComponentProvider : IDamageComponentProvider
{
    private Dictionary<Type, DamageComponentBase> typeToComponent;
    public DefaultDamageComponentProvider(IEnumerable<DamageComponentBase> components)
    {
        typeToComponent = components.ToDictionary(it => it.GetType(), it => it);
    }

    public bool TryGetService<T>(out T component) where T : DamageComponentBase
    {
        if (typeToComponent.TryGetValue(typeof(T), out var result))
        {
            component = (T)result;
            return true;
        }
        component = default;
        return false;
    }
}
