using System;
using System.Linq;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleRawView : MonoBehaviour
{
    [field: SerializeField] public ClickableImage[] PeriodImage { get; private set;}
    [field: SerializeField] public TextMeshProUGUI VillagerText { get; private set;}
    [SerializeField] private Button _saveScheduleButton;
    
    private ActivityColorView _activityColorView;
    
    public event Action<string> OnSavedSchedule;
    public event Action<string, int, ActivityColorData> OnPeriodChanged;

    public void Init(ActivityColorView activityColorView)
    {
        _activityColorView = activityColorView;
        _saveScheduleButton.onClick.AddListener(OnClickedSaveButton);

        foreach (var image in PeriodImage)
        {
            image.OnImageTriggered += ChangePeriod;
        }
    }

    private void OnClickedSaveButton()
    {
        OnSavedSchedule?.Invoke(VillagerText.text);
    }

    private void ChangePeriod(Image image)
    {
        var activityData = _activityColorView.ActivityColors.First(activity => activity.ActivityType == _activityColorView.SelectedActivity);
        image.color = activityData.Color;
        var periodHour = PeriodImage.ToList().FindIndex(im => im.Image == image);
        OnPeriodChanged?.Invoke(VillagerText.text, periodHour, activityData);
    }

    private void OnDestroy()
    {
        _saveScheduleButton.onClick.RemoveListener(OnClickedSaveButton);
        foreach (var image in PeriodImage)
        {
            image.OnImageTriggered -= ChangePeriod;
        }
    }
}