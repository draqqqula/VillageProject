using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DefaultDamageComponentProviderFactory : IDamageComponentProviderFactory
{
    public IDamageComponentProvider Create(IEnumerable<DamageData> data)
    {
        return new DefaultDamageComponentProvider(data.SelectMany(it => it.Components));
    }
}
