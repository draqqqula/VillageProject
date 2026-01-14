using UnityEngine;

public class CycleFromTime : MonoBehaviour
{
    [SerializeField] private DayNightController _dayNight;
    [SerializeField] private SunController _sun;
    [SerializeField] private float _scale = 1;

    [field: SerializeField, Range(0, 1)] public float Time { get; private set; }

    public void SetTime(float value)
    {
        Time = Mathf.Repeat(value, 1);
        UpdateControllers();
    }

    private void UpdateControllers()
    {
        var t = Time * _scale;
        _dayNight.SetTime(t);
        _sun.SetTime(t);
    }
}
