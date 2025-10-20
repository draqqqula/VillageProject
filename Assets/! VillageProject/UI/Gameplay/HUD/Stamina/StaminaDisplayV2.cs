using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StaminaDisplayV2 : SignalListener<StaminaSignalInvoker.StaminaHoverSignal, StaminaSignalInvoker.StaminaChangedSignal>
{
    private const float FILLING_TIME = 0.2f;
    private const string VAR_NAME = "_FillAmount";
    
    [SerializeField] private Image _display;
    [SerializeField] private Image _spendableBg;
    
    private Material _displayFillMaterial;
    private Material _bgFillMaterial;
    
    private Coroutine _coroutine;

    private void Awake()
    {
        _displayFillMaterial = new Material(_display.material);
        _display.material = _displayFillMaterial;
        
        _bgFillMaterial = new Material(_spendableBg.material);
        _spendableBg.material = _bgFillMaterial;
    }

    protected override void OnSignal(StaminaSignalInvoker.StaminaHoverSignal value)
    {
        if (value.Amount == 0) return;
        var fillAmount = _displayFillMaterial.GetFloat(VAR_NAME);
        _displayFillMaterial.SetFloat(VAR_NAME, fillAmount - value.Amount);
        _displayFillMaterial.SetInt("_WithEdge", 0);
    }
    
    protected override void OnSignal(StaminaSignalInvoker.StaminaChangedSignal value)
    {
        if (_bgFillMaterial.GetFloat(VAR_NAME) > value.Value)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(ChangeSpendableBgRoutine(value.Value));
        }
        else _bgFillMaterial.SetFloat(VAR_NAME, value.Value);
        
        _displayFillMaterial.SetFloat(VAR_NAME, value.Value);
    }

    private IEnumerator ChangeSpendableBgRoutine(float toValue)
    {
        var progress = 0f;
        var fromValue = _bgFillMaterial.GetFloat(VAR_NAME);
        
        while (progress < FILLING_TIME)
        {
            progress += Time.deltaTime;
            _bgFillMaterial.SetFloat(VAR_NAME, Mathf.Lerp(fromValue, toValue, progress / FILLING_TIME)); 
            yield return null;
        }
        _displayFillMaterial.SetInt("_WithEdge", 1);
    }
}
