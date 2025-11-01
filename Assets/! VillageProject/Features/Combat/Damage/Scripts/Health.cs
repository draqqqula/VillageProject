using Microsoft.Extensions.DependencyInjection;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Health : MonoBehaviour, IHealth
{
    private IDamageComponentProviderFactory _factory = new DefaultDamageComponentProviderFactory();
    private ReactiveProperty<float> _amount = new ReactiveProperty<float>();

    public event Action<float> OnDamageDealt;

    [field: SerializeField] public DamageData Data { get; private set; }
    public float Amount
    {
        get
        {
            return _amount.Value;
        }
        set
        {
            _amount.Value = value;
        }
    }

    [field: SerializeField] public float MaxAmount { get; private set; }
    
    public IServiceProvider ComponentProvider { get; private set; }
    public ReadOnlyReactiveProperty<float> AmountReactive => _amount;
    
    public bool Deal(DamageSource damage)
    {
        using var scopeA = damage.ComponentProvider.CreateScope();
        using var scopeB = ComponentProvider.CreateScope();

        var context = new DamageContext(scopeA.ServiceProvider, scopeB.ServiceProvider, Amount);

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
        var serviceCollection = new ServiceCollection();
        Data.RegisterTo(serviceCollection);
        ComponentProvider = serviceCollection.BuildServiceProvider();
        _amount.Value = MaxAmount;
    }
}
