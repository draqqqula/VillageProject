using UnityEngine;
using UnityEngine.UI;

public class SyncSettings : MonoBehaviour
{
    [SerializeField] private SwingConfiguration _swingConfiguration;
    [SerializeField] private MovementConfiguration _movementConfiguration;
    [SerializeField] private Slider _sensitivity;
    [SerializeField] private Slider _shiftSensitivityMultiplier;
    [SerializeField] private Slider _cursorSensitivity;
    [SerializeField] private Slider _cursorSmoothing;
    [SerializeField] private Toggle _invert;

    private void OnEnable()
    {
        _shiftSensitivityMultiplier.value = _swingConfiguration.ShiftSensitivityMultiplier;
        _shiftSensitivityMultiplier.onValueChanged.AddListener(HandleShiftSensitivity);

        _cursorSensitivity.value = _swingConfiguration.CursorSensitivity;
        _cursorSensitivity.onValueChanged.AddListener(HandleCursorSensitivity);

        _cursorSmoothing.value = _swingConfiguration.CursorSmoothTime;
        _cursorSmoothing.onValueChanged.AddListener(HandleCursorSmoothTime);

        _sensitivity.value = _movementConfiguration.Sensitivity;
        _sensitivity.onValueChanged.AddListener(HandleSensitivity);

        _invert.isOn = _movementConfiguration.Invert;
        _invert.onValueChanged.AddListener(HandleInvert);
    }

    private void OnDisable()
    {
        _sensitivity.onValueChanged.RemoveAllListeners();
        _shiftSensitivityMultiplier.onValueChanged.RemoveAllListeners();
        _cursorSensitivity.onValueChanged.RemoveAllListeners();
        _cursorSmoothing.onValueChanged.RemoveAllListeners();
    }

    private void HandleShiftSensitivity(float value)
    {
        _swingConfiguration.ShiftSensitivityMultiplier = value;
    }

    private void HandleCursorSensitivity(float value)
    {
        _swingConfiguration.CursorSensitivity = value;
    }

    private void HandleCursorSmoothTime(float value)
    {
        _swingConfiguration.CursorSmoothTime = value;
    }

    private void HandleSensitivity(float value)
    {
        _movementConfiguration.Sensitivity = value;
        _movementConfiguration.Updated.Value += 1;
    }

    private void HandleInvert(bool value)
    {
        _movementConfiguration.Invert = value;
    }
}