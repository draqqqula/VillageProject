using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour
{
    private IDamageComponentProviderFactory _factory = new DefaultDamageComponentProviderFactory();
    private ReactiveProperty<float> _amount = new ReactiveProperty<float>();

    public event Action<float> OnDamageDealt;

    [field: SerializeField] public DamageData Data { get; private set; }
    [field: SerializeField] public float Amount { get; private set; }
    public IDamageComponentProvider ComponentProvider { get; private set; }
    public ReadOnlyReactiveProperty<float> AmountReactive => _amount;


    public bool Deal(DamageSource damage)
    {
        var context = new DamageContext(damage.ComponentProvider, ComponentProvider, Amount);

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
            _amount.Value = Amount;
            OnDamageDealt?.Invoke(amount);
            return true;
        }
        return false;
    }

    private void Awake()
    {
        IEnumerable<DamageData> GetData()
        {
            yield return Data;
        }

        ComponentProvider = _factory.Create(GetData());
        _amount.Value = Amount;
    }
}
