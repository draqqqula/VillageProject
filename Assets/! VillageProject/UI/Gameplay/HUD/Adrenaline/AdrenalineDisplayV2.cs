using System;
using UnityEngine;

public class AdrenalineDisplayV2 : SignalListener<StaminaSignalInvoker.AdrenalineStageChangedSignal>
{
    [SerializeField] private GameObject[] _adrenalineStates;
    private GameObject _currentAdrenaline;

    private void Awake()
    {
        foreach (var adrenalineState in _adrenalineStates)
        {
            adrenalineState.SetActive(false);
        }
    }

    protected override void OnSignal(StaminaSignalInvoker.AdrenalineStageChangedSignal value)
    {
        UpdateAdrenaline(value.Stage);
    }

    private void UpdateAdrenaline(int stage)
    {
        if (_currentAdrenaline != null) _currentAdrenaline.SetActive(false);
        _currentAdrenaline = _adrenalineStates[stage];
        _currentAdrenaline.SetActive(true);
    }
}