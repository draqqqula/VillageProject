using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AdrenalineDisplay : SignalListener<StaminaSignalInvoker.AdrenalineChangedSignal>
{
    [SerializeField] private Image _display;

    protected override void OnSignal(StaminaSignalInvoker.AdrenalineChangedSignal value)
    {
        _display.fillAmount = value.Value;
    }
}
