using System;
using TMPro;
using UnityEngine;
using Zenject;

public class TimeView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [Inject] private GameTimer _gameTimer;

    private void Awake()
    {
        _gameTimer.OnTimeUpdated += UpdateTime;
    }

    public void UpdateTime()
    {
        _timeText.text = GetFormattedTime();
    }
    
    public string GetFormattedTime()
    {
        return $"День {_gameTimer.CurrentDay} {_gameTimer.CurrentHour:00}:{_gameTimer.CurrentMinute:00}";
    }

    private void OnDestroy()
    {
        _gameTimer.OnTimeUpdated -= UpdateTime;
    }
}