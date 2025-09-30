using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDamageComponentProvider
{
    public bool TryGetService<T>(out T component) where T : DamageComponentBase;
}
