using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IDamageExecutable
{
    public int GetPriority();
    public bool TryExecute(DamageInteractionContext context);
}
