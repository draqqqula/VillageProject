using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : InputListener
{
    [SerializeField, FromInputActionAsset("Look")] private InputActionReference _look;
    [SerializeField] private Transform _head;
    [SerializeField] private float _FOVLimitX;
    [SerializeField] private float _sensitivity;
    private float _yRotation;
    private float _xRotation;

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
        var delta = _look.action.ReadValue<Vector2>() * _sensitivity;
        _xRotation = Mathf.Clamp(_xRotation - delta.y, -_FOVLimitX, _FOVLimitX);
        _yRotation += delta.x;
        transform.rotation = Quaternion.Euler(0, _yRotation, 0);
        _head.localRotation = Quaternion.Euler(_xRotation, 0, 0);
    }
}
