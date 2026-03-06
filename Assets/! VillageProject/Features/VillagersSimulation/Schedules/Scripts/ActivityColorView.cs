using System;
using UnityEngine;
using UnityEngine.UI;

public class ActivityColorView : MonoBehaviour
{
    [field: SerializeField] public ActivityColorData[] ActivityColors { get; private set;}
    [field: SerializeField] public ActivityType SelectedActivity { get; private set;}
    [SerializeField] private Image _outline;

    private void Awake()
    {
        foreach (var activityColor in ActivityColors)
        {
            activityColor.Button.image.color = activityColor.Color;
            activityColor.Button.onClick.AddListener(() => ChangeActivity(activityColor));
        }
    }

    private void ChangeActivity(ActivityColorData activityData)
    {
        SelectedActivity = activityData.ActivityType;
        _outline.transform.position = activityData.Button.transform.position;
    }

    private void OnDestroy()
    {
        foreach (var activityColor in ActivityColors)
        {
            activityColor.Button.onClick.RemoveAllListeners();
        }
    }
}

[Serializable]
public class ActivityColorData
{
    [field: SerializeField] public ActivityType ActivityType { get; private set;}
    [field: SerializeField] public Color Color { get; private set;}
    [field: SerializeField] public Button Button { get; private set;}
}