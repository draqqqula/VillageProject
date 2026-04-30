using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class FirstPersonController : InputListener, IInitializable, IDialogueTarget
{
    [SerializeField, FromInputActionAsset("Look")] private InputActionReference _look;
    [SerializeField] private Transform _head;
    [SerializeField] private float _FOVLimitX;
    [SerializeField] private float _sensitivity;
    private float _yRotation;
    private float _xRotation;

    public ModifiableValue<float> Sensitivity { get; private set; }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        var delta = _look.action.ReadValue<Vector2>() * Sensitivity.Value.CurrentValue;
        _xRotation = Mathf.Clamp(_xRotation - delta.y, -_FOVLimitX, _FOVLimitX);
        _yRotation += delta.x;
        transform.rotation = Quaternion.Euler(0, _yRotation, 0);
        _head.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }

    public void Initialize()
    {
        Sensitivity = new ModifiableValue<float>(_sensitivity);
    }

    public void UpdateRotation(Vector3 transformRotation, Vector3 headRotation)
    {
        _xRotation = headRotation.x;
        _yRotation = transformRotation.y;
    }
}
