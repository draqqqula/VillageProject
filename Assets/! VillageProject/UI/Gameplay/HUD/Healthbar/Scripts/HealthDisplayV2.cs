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
        for (int i = 0; i < _healthsImages.Length; i++)
        {
            if (i < Mathf.Floor(amount))
            {
                _healthsImages[i].sprite = _normalHealth;
                _healthsImages[i].enabled = true;
                continue;
            }

            if (i == Mathf.Floor(amount) && amount % 1 != 0)
            {
                _healthsImages[i].sprite = _woundedHealth;
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
