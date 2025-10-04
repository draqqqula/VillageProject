using System.Collections;
using UnityEngine;
using Zenject;
using R3;

public class PlayerHealthSignalInvoker : IInitializable
{
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
        health.OnDamageDealt += it => _signalBus.Fire(new PlayerHurtSignal() { Damage = it });
        health.AmountReactive.Subscribe(it => _signalBus.Fire(new PlayerHealthChangedSignal() { Amount = it }));
        deathEvent.FiredEvent += () => _signalBus.Fire(new PlayerDeathSignal());
    }
}