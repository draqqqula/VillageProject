using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StaminaDisplayV2 : SignalListener<StaminaSignalInvoker.StaminaHoverSignal, StaminaSignalInvoker.StaminaChangedSignal>
{
    private const float FILLING_TIME = 0.2f;
    
    [SerializeField] private Image _display;
    [SerializeField] private Image _spendableBg;
    
    private Coroutine _coroutine;

    protected override void OnSignal(StaminaSignalInvoker.StaminaHoverSignal value)
    {
        if (value.Amount == 0) return;
        _display.fillAmount -= value.Amount;
    }
    
    protected override void OnSignal(StaminaSignalInvoker.StaminaChangedSignal value)
    {
        if (_spendableBg.fillAmount > value.Value)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(ChangeSpendableBgRoutine(value.Value));
        }
        else _spendableBg.fillAmount = value.Value;
        
        _display.fillAmount = value.Value;
    }

    private IEnumerator ChangeSpendableBgRoutine(float toValue)
    {
        var progress = 0f;
        var fromValue = _spendableBg.fillAmount;
        
        while (progress < FILLING_TIME)
        {
            progress += Time.deltaTime;
            _spendableBg.fillAmount = Mathf.Lerp(fromValue, toValue, progress / FILLING_TIME);
            yield return null;
        }
    }
}
