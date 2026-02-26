using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveCallView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _progressImage;
    [SerializeField] private GameObject _progressHolder;
    [SerializeField] private WaveController _waveController;

    private void Awake()
    {
        _waveController.IsOnBreak.Subscribe(OnWaveStateChanged).AddTo(this);
    }

    private void OnWaveStateChanged(bool isEndedWave)
    {
        if (isEndedWave) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }

    public void UpdateProgress(float progress)
    {
        if (progress > 0) _progressHolder.SetActive(true);
        else _progressHolder.SetActive(false);
        
        _progressImage.fillAmount = progress;
    }
}