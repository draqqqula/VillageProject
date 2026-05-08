using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WaveCallView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _progressImage;
    [SerializeField] private GameObject _progressHolder;

    public void ActivateView()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateView()
    {
        gameObject.SetActive(false);
    }
    
    public void UpdateProgress(float progress)
    {
        if (progress > 0) _progressHolder.SetActive(true);
        else _progressHolder.SetActive(false);
        
        _progressImage.fillAmount = progress;
    }
}