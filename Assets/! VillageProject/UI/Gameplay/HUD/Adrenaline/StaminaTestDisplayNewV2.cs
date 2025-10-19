using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaTestDisplayNewV2 : SignalListener<StaminaSignalInvoker.StaminaHoverSignal>
{
    [SerializeField] private Image _spendableBg;
    private Coroutine _coroutine;
    
    protected override void OnSignal(StaminaSignalInvoker.StaminaHoverSignal value)
    {
        _spendableBg.fillAmount -= value.Amount;
    }
}