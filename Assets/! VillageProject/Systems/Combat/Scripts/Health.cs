using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour
{
    private IDamageComponentProvider _componentProvider;
    [SerializeReference, SubclassSelector] public List<DamageComponentBase> Components;
    [field: SerializeField] public float Amount { get; private set; }

    public void Take(DamageInfo damage)
    {
        var context = new DamageContext(damage.ComponentProvider, _componentProvider);

        if (damage.Conditions.All(it => it.IsSatisfied(context)))
        {
            var amount = damage.BaseAmount;
            foreach (var effect in damage.Effects)
            {
                amount = effect.Apply(context, amount);
            }
            Amount -= amount;
        }
    }

    private void Awake()
    {
        _componentProvider = new DefaultDamageComponentProvider(Components);
    }
}
