using TMPro;
using UnityEngine;

public class TimeView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private GameTimer _gameTimer;

    public void UpdateTime()
    {
        _timeText.text = GetFormattedTime();
    }
    
    public string GetFormattedTime()
    {
        return $"Day {_gameTimer.CurrentDay} {_gameTimer.CurrentHour:00}:{_gameTimer.CurrentMinute:00}";
    }
}