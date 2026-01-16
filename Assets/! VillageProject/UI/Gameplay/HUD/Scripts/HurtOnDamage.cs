using R3;
using UnityEngine;
using Zenject;

public class HurtOnDamage : SignalListener<PlayerHealthSignalInvoker.PlayerHurtSignal>
{
    [SerializeField] private HurtEffect _hurt;

    protected override void OnSignal(PlayerHealthSignalInvoker.PlayerHurtSignal amount)
    {
        if (amount.Damage <= 0)
        {
            return;
        }
        _hurt.Show();
    }
}
