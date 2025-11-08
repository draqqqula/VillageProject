using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class StaminaDisplay : SignalListener<StaminaSignalInvoker.StaminaChangedSignal>
{
    [SerializeField] private Image _display;

    protected override void OnSignal(StaminaSignalInvoker.StaminaChangedSignal value)
    {
        _display.fillAmount = value.Value;
    }
}
