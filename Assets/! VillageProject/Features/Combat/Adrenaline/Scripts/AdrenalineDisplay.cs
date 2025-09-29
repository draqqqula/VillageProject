using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AdrenalineDisplay : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] private Image _display;

    private void Awake()
    {
        _signalBus.Subscribe<StaminaSignalInvoker.AdrenalineChangedSignal>(it => UpdateDisplay(it.Value));
    }

    private void UpdateDisplay(float value)
    {
        _display.fillAmount = value;
    }
}
