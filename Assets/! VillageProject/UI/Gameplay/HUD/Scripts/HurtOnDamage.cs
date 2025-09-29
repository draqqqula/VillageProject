using R3;
using UnityEngine;
using Zenject;

public class HurtOnDamage : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] private HurtEffect _hurt;

    private void Awake()
    {
        _signalBus.Subscribe<PlayerHealthSignalInvoker.PlayerHurtSignal>(it => HandleDamageDealt(it.Damage));
    }

    private void HandleDamageDealt(float amount)
    {
        _hurt.Show();
    }
}
