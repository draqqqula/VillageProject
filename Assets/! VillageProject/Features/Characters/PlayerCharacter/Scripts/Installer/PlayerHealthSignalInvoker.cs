using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerHealthSignalInvoker : MonoBehaviour
{
    public class PlayerHurtSignal
    {
        public float Damage;
    }

    public class PlayerDeathSignal
    {
    }

    [Inject(Source = InjectSources.Parent)] private SignalBus _signalBus;
    [Inject] private Health _health;
    [Inject] private DeathEvent _deathEvent;

    private void Awake()
    {
        _signalBus.DeclareSignal<PlayerHurtSignal>();
        _signalBus.DeclareSignal<PlayerDeathSignal>();
        _health.OnDamageDealt += it => _signalBus.Fire(new PlayerHurtSignal() { Damage = it });
        _deathEvent.FiredEvent += () => _signalBus.Fire(new PlayerDeathSignal());
    }
}