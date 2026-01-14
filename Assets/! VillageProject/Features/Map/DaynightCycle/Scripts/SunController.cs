using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private AnimationCurve _progressOverTime;
    [SerializeField] private Gradient _lightColorOverTime;
    [SerializeField] private float _minAngle = 0;
    [SerializeField] private float _maxAngle = 180;
    [SerializeField] private float _offsetAngle = 90;

    public float Time { get; private set; }

    public void SetTime(float value)
    {
        var t = Mathf.Repeat(value, 1);
        Time = t;
        UpdateSun();
    }

    public void UpdateSun()
    {
        _light.color = _lightColorOverTime.Evaluate(Time);
        RotateSun(_progressOverTime.Evaluate(Time));
    }

    public void RotateSun(float progress)
    {
        var angle = Mathf.Repeat(
            _offsetAngle + Mathf.Lerp(_minAngle, _maxAngle, progress) - _minAngle,
            _maxAngle - _minAngle
        ) + _minAngle;
        _light.transform.eulerAngles = new Vector3(angle, 0, 0);
    }
}
