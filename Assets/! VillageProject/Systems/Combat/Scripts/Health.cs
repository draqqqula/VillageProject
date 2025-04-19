using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour
{
    private IDamageComponentProviderFactory _factory = new DefaultDamageComponentProviderFactory();
    [field: SerializeField] public DamageData Data { get; private set; }
    [field: SerializeField] public float Amount { get; private set; }
    public IDamageComponentProvider ComponentProvider { get; private set; }

    public void Deal(DamageSource damage)
    {
        var context = new DamageContext(damage.ComponentProvider, ComponentProvider);

        if (damage.Data.Conditions.All(it => it.IsSatisfied(context))
            && damage.Info.Data.Conditions.All(it => it.IsSatisfied(context))
            && Data.Conditions.All(it => it.IsSatisfied(context)))
        {
            var amount = damage.Info.BaseAmount;
            foreach (var effect in damage.Data.Effects
                .Concat(damage.Info.Data.Effects)
                .Concat(Data.Effects))
            {
                amount = effect.Apply(context, amount);
            }
            Amount -= amount;
        }
    }

    private void Awake()
    {
        IEnumerable<DamageData> GetData()
        {
            yield return Data;
        }

        ComponentProvider = _factory.Create(GetData());
    }
}
