using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class IndicatorController : MonoBehaviour
{
    private Camera _targetCamera;
    
    [Header("Thresholds")]
    [Tooltip("Максимальная дистанция до курсора")]
    [SerializeField, Range(0.01f, 0.5f)] private float _thresholdToCursor = 0.2f;
    [Tooltip("Максимальная дистанция до камеры")]
    [SerializeField, Range(1, 15)] private float _thresholdToCamera = 10;
    private float _lastThresholdToCamera = -1;
    
    [Tooltip("График зависимости порога дистанции до курсора от дистанции до камеры")]
    [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
    [Tooltip("Перерисовывать ли график при изменении длины осей?")]
    [SerializeField] private bool _isAutoRedrawCurve = true;

    [Header("Multipliers")]
    [Tooltip("Множитель веса расстояния до камеры")]
    [SerializeField] private float _cameraWeightMultiplier = 1;
    [Tooltip("Множитель веса расстояния до курсора")]
    [SerializeField] private float _cursorWeightMultiplier = 1;
    [Tooltip("Учитывать график при расчете веса")]
    [SerializeField] private bool _isConsiderCurve = true;

    [Header("Additive settings")] 
    [Tooltip("Переключает на режим строгого закрепления индикатора")]
    [SerializeField] private bool _isLockIndicator = false;
    [Tooltip("Активирует дебаг режим")]
    [SerializeField] private bool _isActivateDebug;
    
    private const float ANGLE_THRESHOLD = 90f;
    
    [SerializeField] private GameObject _indicatorObject;
    private IndicatorActivator _indicatorActivator;
    private List<Origin> _origins = new List<Origin>();
    
    private void Awake()
    {
        _targetCamera = Camera.main;
        _indicatorActivator = new IndicatorActivator(_indicatorObject, _targetCamera);
    }

    public void AddOrigin(Origin origin)
    {
        _origins.Add(origin);
    }

    public void RemoveOrigin(Origin origin)
    {
        if (_origins.Contains(origin)) _origins.Remove(origin);
    }
    
    private void Update()
    {
        Origin origin = GetLockedOrigin();
        
        if (origin != null) _indicatorActivator.ActivateIndicator(origin);
        else _indicatorActivator.DeactivateIndicator();
        
        _indicatorActivator.UpdateIndicator();
        
        #if UNITY_EDITOR
        if (_isActivateDebug) PrintDebug();
        #endif
    }
    
    private Origin GetLockedOrigin()
    {
        if (_isLockIndicator && _indicatorActivator.LockedOrigin != null) return _indicatorActivator.LockedOrigin;
        
        var maxWeight = float.MinValue;
        Origin result = null;

        foreach (var origin in _origins)
        {
            if (!IsCameraLooking(origin.OriginPoint.position) || IsHaveObstacles(origin)) continue;
            
            var distanceToCamera = GetDistanceToCamera(origin.OriginPoint.position);
            var distanceToCursor = GetDistanceToCursor(origin.OriginPoint.position);
            
            var weightToCamera = Mathf.InverseLerp(_thresholdToCamera, 0, distanceToCamera) * _cameraWeightMultiplier;
            float weightToCursor = 0;

            if (_curve != null && _isConsiderCurve)
            {
                weightToCursor = Mathf.InverseLerp(0, _thresholdToCursor * _curve.Evaluate(distanceToCamera), distanceToCursor) * _cursorWeightMultiplier;
            }
            else weightToCursor = Mathf.InverseLerp(0, _thresholdToCursor, distanceToCursor) * _cursorWeightMultiplier;
            
            var weight = weightToCursor + weightToCamera;
            
            if (weight > maxWeight)
            {
                maxWeight = weight;
                result = origin;
            }
        }
        
        return result;
    }

    private bool IsHaveObstacles(Origin origin)
    {
        var direction = (origin.OriginPoint.position - _targetCamera.transform.position).normalized;
        var distance = GetDistanceToCamera(origin.OriginPoint.position);
        
        if (Physics.Raycast(_targetCamera.transform.position, direction, out var hit, distance,
                ~LayerMask.GetMask("Enemy", "Bodies", "TargetDetector", "Ignore Raycast")))
        {
            return true;
        }
        return false;
    }
    
    private bool IsCameraLooking(Vector3 point)
    {
        if (!IsRotatedToTarget(_targetCamera.transform, point)) return false;
        
        float distanceToCursor = GetDistanceToCursor(point);
        float distanceToCamera = GetDistanceToCamera(point);
        
        if (_curve != null) return distanceToCursor <= _thresholdToCursor * _curve.Evaluate(distanceToCamera);
        return distanceToCursor <= _thresholdToCamera;
    }
    
    private bool IsRotatedToTarget(Transform cameraTransform, Vector3 targetPos, float angleThreshold = ANGLE_THRESHOLD)
    {
        Vector3 directionToTarget = (targetPos - cameraTransform.position).normalized;
        Vector3 cameraForward = cameraTransform.forward;
        float angle = Vector3.Angle(cameraForward, directionToTarget);
        
        return angle <= angleThreshold;
    }
    
    private float GetDistanceToCursor(Vector3 point)
    {
        Vector3 viewportPoint = _targetCamera.WorldToViewportPoint(point);
        
        return Vector2.Distance(
            new Vector2(viewportPoint.x, viewportPoint.y), 
            new Vector2(0.5f, 0.5f)
        );
    }

    private float GetDistanceToCamera(Vector3 point)
    {
        Vector3 viewportPoint = _targetCamera.WorldToViewportPoint(point);
        return Mathf.Clamp(viewportPoint.z, 0, _thresholdToCamera);
    }

#if UNITY_EDITOR
    private void PrintDebug()
    {
        if (_indicatorActivator.LockedOrigin == null) return;
        
        var distance = Vector3.Distance(_targetCamera.transform.position, _indicatorActivator.LockedOrigin.OriginPoint.position);
        Debug.Log($"Distance from camera to origin: {distance}");

        Vector3 directionToPoint = (_indicatorActivator.LockedOrigin.OriginPoint.position - _targetCamera.transform.position).normalized;
        float angle = Vector3.Angle(_targetCamera.transform.forward, directionToPoint);
        Debug.Log($"Angle from cursor to origin: {angle}");
    }
    
    private void OnValidate()
    {
        if (_curve.length < 2) Debug.LogError("Curve has less than 2 keys!");
        _curve.MoveKey(0, new Keyframe(0, _curve[0].value));
        
        if (_curve[_curve.length - 1].time < _thresholdToCamera)
            _curve.MoveKey(_curve.length - 1, new Keyframe(_thresholdToCamera, _curve[_curve.length - 1].value));
        
        if (!_isAutoRedrawCurve || Mathf.Approximately(_lastThresholdToCamera, _thresholdToCamera)) return;
        if (_lastThresholdToCamera == -1)
        {
            _lastThresholdToCamera = _thresholdToCamera;
            return;
        }
        
        RedrawCurveWithNewParams();
        _lastThresholdToCamera = _thresholdToCamera;
    }

    [ContextMenu("Redraw Curve")]
    private void RedrawCurveWithNewParams()
    {
        var lastCurve = _curve;
        _curve = AnimationCurve.Linear(0, _curve[0].value, _thresholdToCamera, _curve[_curve.length - 1].value);
        
        if (lastCurve != null)
        {
            int count = 0;
            foreach (var oldKey in lastCurve.keys)
            {
                if (Mathf.Approximately(oldKey.value, _curve[_curve.length - 1].value)) continue;
                
                var newKey = new Keyframe(oldKey.time / _lastThresholdToCamera * _thresholdToCamera, 
                    oldKey.value, oldKey.outTangent, oldKey.inTangent);
                
                if (_curve.length >= count) _curve.AddKey(newKey);
                else _curve.MoveKey(count, newKey);
                count++;
            }
        }
    }
    #endif
}