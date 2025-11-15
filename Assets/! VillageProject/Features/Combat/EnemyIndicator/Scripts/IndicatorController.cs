using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.UI.Image;

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
    
    private const float ANGLE_THRESHOLD = 90f;
    
    [SerializeField] private GameObject _indicatorObject;

    private ReactiveProperty<Vector2> _vectorToCursor = new ReactiveProperty<Vector2>(Vector2.zero);
    private IndicatorActivator _indicatorActivator;
    private List<Origin> _origins = new List<Origin>();

    public ReadOnlyReactiveProperty<Vector2> VectorToCursor => _vectorToCursor;

    
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
        Origin origin = GetLockedOrigin(out var vectorToCursor);
        _vectorToCursor.Value = vectorToCursor;
        
        if (origin != null) _indicatorActivator.ActivateIndicator(origin);
        else _indicatorActivator.DeactivateIndicator();
        
        _indicatorActivator.UpdateIndicator();
    }
    
    private Origin GetLockedOrigin(out Vector2 vectorToCursor)
    {
        if (_isLockIndicator && _indicatorActivator.LockedOrigin != null)
        {
            var point = _indicatorActivator.LockedOrigin.OriginPoint.position;
            GetDistanceToCameraAndVectorToCursor(point, out var distanceToCamera, out vectorToCursor);
            return _indicatorActivator.LockedOrigin;
        }

        var maxWeight = float.MinValue;
        var resultingVectorToCursor = Vector2.zero;
        Origin result = null;

        foreach (var origin in _origins)
        {
            var point = origin.OriginPoint.position;
            if (!IsCameraLooking(point) || IsHaveObstacles(point)) continue;

            GetDistanceToCameraAndVectorToCursor(point, out var distanceToCamera, out var normalizedVectorToCursor);

            var weightToCamera = Mathf.InverseLerp(_thresholdToCamera, 0, distanceToCamera) * _cameraWeightMultiplier;
            var weightToCursor = (1 - normalizedVectorToCursor.magnitude) * _cursorWeightMultiplier;
            
            var weight = weightToCursor + weightToCamera;
            
            if (weight > maxWeight)
            {
                resultingVectorToCursor = normalizedVectorToCursor;
                maxWeight = weight;
                result = origin;
            }
        }

        vectorToCursor = resultingVectorToCursor;
        return result;
    }

    private void GetDistanceToCameraAndVectorToCursor(Vector3 point, out float distanceToCamera, out Vector2 vectorToCursor)
    {
        distanceToCamera = GetDistanceToCamera(point);
        var thresholdToCursor = GetThresholdToCursor(distanceToCamera);
        vectorToCursor = GetNormalizedVectorToCursor(point, thresholdToCursor);
    }

    private bool IsHaveObstacles(Vector3 point)
    {
        var direction = (point - _targetCamera.transform.position).normalized;
        var distance = GetDistanceToCamera(point);
        
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

    private Vector2 GetVectorToCursor(Vector3 point)
    {
        Vector3 viewportPoint = _targetCamera.WorldToViewportPoint(point);

        return new Vector2(viewportPoint.x, viewportPoint.y) - new Vector2(0.5f, 0.5f);
    }

    private Vector2 GetNormalizedVectorToCursor(Vector3 point, float thresholdToCursor)
    {
        var unclamped = GetVectorToCursor(point) / thresholdToCursor;
        return Vector2.ClampMagnitude(unclamped, 1);
    }

    private float GetThresholdToCursor(float distanceToCamera)
    {
        if (!_isConsiderCurve)
        {
            return _thresholdToCursor;
        }
        var normalizedDistanceToCamera = Mathf.InverseLerp(0, _thresholdToCamera, distanceToCamera);
        return _thresholdToCursor * _curve.Evaluate(normalizedDistanceToCamera);
    }
}