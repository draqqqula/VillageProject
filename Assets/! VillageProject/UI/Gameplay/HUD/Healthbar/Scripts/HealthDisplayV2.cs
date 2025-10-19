using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(AlignerByPixels))]
public class HealthDisplayV2 : SignalListener<PlayerHealthSignalInvoker.PlayerHealthChangedSignal, PlayerHealthSignalInvoker.PlayerInitializeHealthSignal>
{
    private List<Image> _healthsImages = new List<Image>();

    [SerializeField] private Image _heartPrefab;
    [SerializeField] private Sprite _normalHealth;
    [SerializeField] private Sprite _woundedHealth;

    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    
    private AlignerByPixels _alignerByPixels;
    
    private bool _isInitialized = false;
    
    private void Init(float amount)
    {
        if (_isInitialized) return;
        
        _alignerByPixels = GetComponent<AlignerByPixels>();
        SpawnHearts(amount);
        AlignHearts(_healthsImages);
        _isInitialized = true;
    }
    
    private void SpawnHearts(float amount)
    {
        for (int i = 0; i < amount / 2; i++)
        {
            var heart = Instantiate(_heartPrefab, Vector3.zero, Quaternion.identity, transform);
            _healthsImages.Add(heart);
        }
    }
    
    private void AlignHearts(List<Image> hearts)
    {
        var width = _endPoint.localPosition.x - _startPoint.localPosition.x;
        var heartWidth = hearts[0].rectTransform.rect.width;
        
        var totalHeartsWidth = heartWidth * hearts.Count;
        var totalGapSpace = width - totalHeartsWidth;
        var gapBetweenHearts = totalGapSpace / (hearts.Count - 1);
        
        var counter = 0;
        foreach (var heart in hearts)
        {
            if (counter % 2 == 0)
                heart.rectTransform.localPosition = new Vector2(_startPoint.localPosition.x, _endPoint.localPosition.y);
            else heart.rectTransform.localPosition = _startPoint.localPosition;
            
            heart.rectTransform.localPosition += Vector3.right * (heartWidth + gapBetweenHearts) * counter;
            counter++;
        }
        
        _alignerByPixels.AlignByPixels();
    }
    
    protected override void OnSignal(PlayerHealthSignalInvoker.PlayerInitializeHealthSignal signal)
    {
        Init(signal.Value);
    }
    
    protected override void OnSignal(PlayerHealthSignalInvoker.PlayerHealthChangedSignal signal)
    {
        UpdateHearts(signal.Amount);
    }
    
    private void UpdateHearts(float amount)
    {
        var hearthAmount = amount / 2;
        
        for (int i = 0; i < _healthsImages.Count; i++)
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
}
