using System;
using R3;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using Zenject;

public class CameraShaking : MonoBehaviour
{
    [Header("Horizontal Shake (X Axis)")]
    [SerializeField] private AnimationCurve _xCurve = AnimationCurve.Linear(0, 0, 1, 0);
    [SerializeField, Range(0, 5)] private float _frequencyX = 2;
    [SerializeField, Range(1, 10)] private float _amplitudeMultiplyerX = 5;
    
    [Header("Vertical Shake (Y Axis)")]
    [SerializeField] private AnimationCurve _yCurve = AnimationCurve.Linear(0, 0, 1, 0);
    [SerializeField, Range(0, 10)] private float _frequencyY = 2;
    [SerializeField, Range(1, 10)] private float _amplitudeMultiplyerY = 5;
    
    [Space]
    [SerializeField] private Transform _offset;
    private Vector3 _velocity;
    
    [Space]
    [SerializeField] private bool _isShakeWithOffset;
    private float _shakeTime = 0f;

    [Inject]
    private void Construct(CharacterVelocity characterVelocity)
    {
        characterVelocity.Velocity.Subscribe(OnVelocityChanged).AddTo(this);
    }

    private void OnVelocityChanged(Vector3 velocity) => _velocity = new Vector3(velocity.x, 0, velocity.z);
    
    private void FixedUpdate()
    {
        Shake(_velocity.magnitude);
    }

    public void Shake() => Shake(1);
    
    public void Shake(float velocity)
    {
        var offsetX = CalculateShakeOffset(velocity, _amplitudeMultiplyerX, _frequencyX, _xCurve);
        var offsetY = CalculateShakeOffset(velocity, _amplitudeMultiplyerY, _frequencyY, _yCurve);
        
        if (_isShakeWithOffset) _offset.localPosition = new Vector3(offsetX, offsetY, 0);
        else transform.localPosition = new Vector3(offsetX, offsetY, 0);
    }

    private float CalculateShakeOffset(float velocity, float amplitude, float frequency, AnimationCurve curve)
    {
        _shakeTime += Time.fixedDeltaTime * frequency * velocity;
        return amplitude * curve.Evaluate(_shakeTime);
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        _xCurve.postWrapMode = WrapMode.Loop;
        _yCurve.postWrapMode = WrapMode.Loop;
    }
    #endif
}
