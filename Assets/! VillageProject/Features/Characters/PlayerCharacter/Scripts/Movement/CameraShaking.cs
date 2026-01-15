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
    private float _moveParam;
    
    [Space]
    [SerializeField] private bool _isShakeWithOffset;
    private float _shakeTime = 0f;

    private MoveParamUpdater _moveParamUpdater;
    
    [Inject]
    private void Construct(MoveParamUpdater moveParamUpdater)
    {
        _moveParamUpdater = moveParamUpdater;
        moveParamUpdater.MoveParameter.Subscribe(OnMoveParamChanged).AddTo(this);
    }

    private void OnMoveParamChanged(float moveParam) => _moveParam = moveParam;
    
    private void FixedUpdate()
    {
        Shake(_moveParam);
    }

    public void Shake() => Shake(0);
    
    public void Shake(float moveParam)
    {
        var offsetX = CalculateShakeOffset(moveParam, _amplitudeMultiplyerX, _frequencyX, _xCurve) * moveParam;
        var offsetY = CalculateShakeOffset(moveParam, _amplitudeMultiplyerY, _frequencyY, _yCurve) * moveParam;
        
        if (_isShakeWithOffset) _offset.localPosition = new Vector3(offsetX, offsetY, 0);
        else transform.localPosition = new Vector3(offsetX, offsetY, 0);
    }

    private float CalculateShakeOffset(float moveParam, float amplitude, float frequency, AnimationCurve curve)
    {
        if (!_moveParamUpdater.IsDecreasing) _shakeTime = frequency * moveParam;
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
