using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StaminaDisplayV2 : SignalListener<StaminaSignalInvoker.StaminaHoverSignal, StaminaSignalInvoker.StaminaChangedSignal>
{
    private const float FILLING_TIME = 0.2f;
    private const string VAR_NAME = "_FillAmount";
    
    [SerializeField] private Image _firstPlanDisplay;
    [SerializeField] private Image _secondPlanDisplay;
    [SerializeField] private Image _thirdPlanDisplay;
    
    private Material _firstDisplayMaterial;
    private Material _secondDisplayMaterial;
    private Material _thirdDisplayMaterial;
    
    private Coroutine _coroutine;

    private void Awake()
    {
        _firstDisplayMaterial = CreateMaterialInstance(_firstPlanDisplay);
        _secondDisplayMaterial = CreateMaterialInstance(_secondPlanDisplay);
        _thirdDisplayMaterial = CreateMaterialInstance(_thirdPlanDisplay);
    }

    private Material CreateMaterialInstance(Image display)
    {
        var material = new Material(display.material);
        display.material = material;
        return material;
    }

    protected override void OnSignal(StaminaSignalInvoker.StaminaHoverSignal value)
    {
        if (value.Amount == 0) return;
        var fillAmount = _secondDisplayMaterial.GetFloat(VAR_NAME);
        
        ChangeFrontPlansDisplays(fillAmount - value.Amount);
        _firstDisplayMaterial.SetInt("_WithEdge", 0);
    }
    
    protected override void OnSignal(StaminaSignalInvoker.StaminaChangedSignal value)
    {
        if (_thirdDisplayMaterial.GetFloat(VAR_NAME) > value.Value)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(ChangeThirdDisplayRoutine(value.Value));
        }
        else _thirdDisplayMaterial.SetFloat(VAR_NAME, value.Value);
        
        ChangeFrontPlansDisplays(value.Value);
    }

    private void ChangeFrontPlansDisplays(float value)
    {
        _firstDisplayMaterial.SetFloat(VAR_NAME, value);
        _secondDisplayMaterial.SetFloat(VAR_NAME, value);
    }

    private IEnumerator ChangeThirdDisplayRoutine(float toValue)
    {
        var progress = 0f;
        var fromValue = _thirdDisplayMaterial.GetFloat(VAR_NAME);
        
        while (progress < FILLING_TIME)
        {
            progress += Time.deltaTime;
            _thirdDisplayMaterial.SetFloat(VAR_NAME, Mathf.Lerp(fromValue, toValue, progress / FILLING_TIME)); 
            yield return null;
        }
        _firstDisplayMaterial.SetInt("_WithEdge", 1);
    }
}
