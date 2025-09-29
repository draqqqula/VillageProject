using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class StaminaDisplay : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [SerializeField] private Image _display;

    private void Awake()
    {
        _signalBus.Subscribe<StaminaSignalInvoker.StaminaChangedSignal>(it => UpdateDisplay(it.Value));
    }

    private void UpdateDisplay(float value)
    {
        _display.fillAmount = value;
    }
}
