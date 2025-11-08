using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDamageInteractable
{
    public DamageRole Role { get; }
    public IServiceProvider AttributeProvider { get; }
}
