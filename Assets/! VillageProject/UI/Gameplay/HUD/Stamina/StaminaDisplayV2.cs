using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaDisplayV2 : SignalListener<StaminaSignalInvoker.StaminaChangedSignal>
{
    private const float FILLING_TIME = 0.5f;
    
    [SerializeField] private Image _display;
    [SerializeField] private Image _spendableBg;
    
    private Coroutine _coroutine;

    protected override void OnSignal(StaminaSignalInvoker.StaminaChangedSignal value)
    {
        if (value.Value < _display.fillAmount)
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
