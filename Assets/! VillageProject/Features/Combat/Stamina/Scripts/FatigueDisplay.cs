using R3;
using UnityEngine;
using Zenject;

public class FatigueDisplay : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] private HurtEffect _hurt;

    private void Awake()
    {
        _signalBus.Subscribe<StaminaSignalInvoker.FatigueSignal>(it => HandleOnCooldown(it.OnCooldown));
    }

    private void HandleOnCooldown(bool value)
    {
        if (value)
        {
            _hurt.Show();
        }
    }
}
