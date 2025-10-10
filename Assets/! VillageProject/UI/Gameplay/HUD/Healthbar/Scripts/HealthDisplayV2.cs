using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayV2 : SignalListener<PlayerHealthSignalInvoker.PlayerHealthChangedSignal>
{
    [SerializeField] private Image[] _healthsImages;
    
    [SerializeField] private Sprite _normalHealth;
    [SerializeField] private Sprite _woundedHealth;
    
    protected override void OnSignal(PlayerHealthSignalInvoker.PlayerHealthChangedSignal signal)
    {
        UpdateHealth(signal.Amount);
    }

    private void UpdateHealth(float amount)
    {
        var hearthAmount = amount / 2;
        
        for (int i = 0; i < _healthsImages.Length; i++)
        {
            if (i < Mathf.Ceil(hearthAmount))
            {
                if (i == Mathf.Ceil(hearthAmount) - 1 && hearthAmount % 1 != 0) _healthsImages[i].sprite = _woundedHealth;
                else _healthsImages[i].sprite = _normalHealth;
                
                _healthsImages[i].enabled = true;
                continue;
            }
            
            _healthsImages[i].enabled = false;
        }
    }

    private void OnValidate()
    {
        _healthsImages = new Image [10];
        for (int i = 0; i < _healthsImages.Length; i++)
        {
            _healthsImages[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }
}
