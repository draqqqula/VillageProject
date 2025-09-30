using R3;
using UnityEngine;
using Zenject;

public class FatigueDisplay : SignalListener<StaminaSignalInvoker.FatigueSignal>
{
    [SerializeField] private HurtEffect _hurt;

    protected override void OnSignal(StaminaSignalInvoker.FatigueSignal value)
    {
        if (value.OnCooldown)
        {
            _hurt.Show();
        }
    }
}
