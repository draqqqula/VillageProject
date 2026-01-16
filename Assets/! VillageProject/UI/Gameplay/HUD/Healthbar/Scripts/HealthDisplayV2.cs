using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(AlignerByPixels))]
public class HealthDisplayV2 : SignalListener<PlayerHealthSignalInvoker.PlayerHealthChangedSignal, PlayerHealthSignalInvoker.PlayerInitializeHealthSignal,
PlayerHealthSignalInvoker.PlayerResetMaxHealthSignal>
{
    private List<HeartView> _healthsImages = new List<HeartView>();

    [SerializeField] private HeartView _heartPrefab;
    
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    
    private AlignerByPixels _alignerByPixels;
    
    private void RespawnHearts(float amount)
    {
        ClearHearts();
        _alignerByPixels = GetComponent<AlignerByPixels>();
        SpawnHearts(amount);
        AlignHearts(_healthsImages);
    }

    private void ClearHearts()
    {
        foreach (var image in _healthsImages)
        {
            Destroy(image.gameObject);
        }
        _healthsImages.Clear();
    }
    
    private void SpawnHearts(float amount)
    {
        for (int i = 0; i < amount / 2; i++)
        {
            var heart = Instantiate(_heartPrefab, Vector3.zero, Quaternion.identity, transform);
            _healthsImages.Add(heart);
        }
    }
    
    private void AlignHearts(List<HeartView> hearts)
    {
        var width = _endPoint.localPosition.x - _startPoint.localPosition.x;
        var heartWidth = hearts[0].RectTransform.rect.width;
        
        var totalHeartsWidth = heartWidth * hearts.Count;
        var totalGapSpace = width - totalHeartsWidth;
        var gapBetweenHearts = hearts.Count > 1 ? totalGapSpace / (hearts.Count - 1) : 1;
        
        var counter = 0;
        foreach (var heart in hearts)
        {
            if (counter % 2 == 0)
                heart.RectTransform.localPosition = new Vector2(_startPoint.localPosition.x, _endPoint.localPosition.y);
            else heart.RectTransform.localPosition = _startPoint.localPosition;
            
            Debug.Log(heart.RectTransform.localPosition);
            Debug.Log((heartWidth + gapBetweenHearts));
            heart.RectTransform.localPosition += Vector3.right * (heartWidth + gapBetweenHearts) * counter;
            counter++;
        }
        
        _alignerByPixels.AlignByPixels();
    }
    
    protected override void OnSignal(PlayerHealthSignalInvoker.PlayerInitializeHealthSignal signal)
    {
        RespawnHearts(signal.Value);
    }
    
    protected override void OnSignal(PlayerHealthSignalInvoker.PlayerResetMaxHealthSignal signal)
    {
        Debug.Log("Reset Max Health");
        RespawnHearts(signal.Value);
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
                if (i == Mathf.Ceil(hearthAmount) - 1 && hearthAmount % 1 != 0) _healthsImages[i].SetWoundedHeart();
                else _healthsImages[i].SetNormalHeart();
                continue;
            }
            
            _healthsImages[i].DisableHeart();
        }
    }
}