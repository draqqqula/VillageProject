using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class FirstPersonController : InputListener, IInitializable
{
    [SerializeField, FromInputActionAsset("Look")] private InputActionReference _look;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _hands;
    
    [SerializeField] private float _FOVLimitX;
    [SerializeField] private float _sensitivity;
    private float _yRotation;
    private float _xRotation;

    public ModifiableValue<float> Sensitivity { get; private set; }

    private const float MaxAngle = 30f;
    [SerializeField] private float _handsDelay = 0.05f;

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
        
        var handsRotation = _hands.rotation;
        transform.rotation = Quaternion.Euler(0, _yRotation, 0);
        _head.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        _hands.rotation = handsRotation;

        SmoothHandsRotation();
    }
    
    private void SmoothHandsRotation()
    {
        Quaternion targetHandsRotation = Quaternion.Euler(_xRotation, 0, 0);
        
        float angleDifference = Quaternion.Angle(_hands.localRotation, targetHandsRotation);
        float progress = Mathf.Clamp01(angleDifference / MaxAngle);
        float smoothSpeed = Mathf.SmoothStep(0.2f, 1, progress);
        
        _hands.localRotation = Quaternion.Slerp(
            _hands.localRotation,
            targetHandsRotation,
            Time.deltaTime / _handsDelay * smoothSpeed
        );
    }

    public void Initialize()
    {
        Sensitivity = new ModifiableValue<float>(_sensitivity);
    }
}
