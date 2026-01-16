using System.Collections;
using UnityEngine;
using Zenject;
using R3;

public class PlayerHealthSignalInvoker : IInitializable
{
    public class PlayerInitializeHealthSignal
    {
        public float Value;
    }
    
    public class PlayerResetMaxHealthSignal
    {
        public float Value;
    }
    
    public class PlayerHurtSignal
    {
        public float Damage;
    }

    public class PlayerHealthChangedSignal
    {
        public float Amount;
    }

    public class PlayerDeathSignal
    {
    }

    [Inject] private SignalBus _signalBus;
    [Inject] private Health health;
    [Inject] private DeathEvent deathEvent;

    public void Initialize()
    {
        _signalBus.Fire(new PlayerInitializeHealthSignal() { Value = health.MaxHealth });
        health.OnDamageDealt += it => _signalBus.Fire(new PlayerHurtSignal() { Damage = it });
        health.AmountReactive.Subscribe(it => _signalBus.Fire(new PlayerHealthChangedSignal() { Amount = it }));
        health.MaxAmountReactive.Subscribe(it => _signalBus.Fire(new PlayerResetMaxHealthSignal() { Value = it }));
        deathEvent.FiredEvent += () => _signalBus.Fire(new PlayerDeathSignal());
    }
}