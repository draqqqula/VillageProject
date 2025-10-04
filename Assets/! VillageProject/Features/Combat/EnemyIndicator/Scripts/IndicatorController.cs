using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class IndicatorController : MonoBehaviour
{
    private Camera _targetCamera;
    
    [SerializeField, Range(0.01f, 0.5f)] private float _thresholdToCursor = 0.2f;
    [SerializeField, Range(1, 15)] private float _thresholdToCamera = 10;
    private float _lastThresholdToCamera;
    
    [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private bool _isAutoRedrawCurve = true;
    
    [SerializeField] private GameObject _indicatorObject;
    private IndicatorActivator _indicatorActivator;
    
    private const float CHECK_INTERVAL = 0.3f;
    private float lastCheckTime;
    
    private void Awake()
    {
        _targetCamera = Camera.main;
        _indicatorActivator = new IndicatorActivator(_indicatorObject, _targetCamera);
    }
    
    private void Update()
    {
        _indicatorActivator.UpdateIndicator();
        
        if (Time.time - lastCheckTime < CHECK_INTERVAL) return;
        var markedObj = GetLookedMarkedObj();
        if (markedObj != null)
        {
            _indicatorActivator.ActivateIndicator(markedObj);
        }
        else _indicatorActivator.DeactivateIndicator();
        
        lastCheckTime = Time.time;
    }
    
    private IMarkedByIndicator GetLookedMarkedObj()
    {
        var markedObjs = CheckMarkedByIndicatorObjs();

        var minDistance = float.MaxValue;
        IMarkedByIndicator lookedObj = null;

        foreach (var markedObj in markedObjs)
        {
            if (!IsCameraLooking(markedObj.OriginPoint.position)) continue;
            var distanceToViewportPoint = GetDistanceToCursor(markedObj.OriginPoint.position);

            if (distanceToViewportPoint < minDistance)
            {
                minDistance = distanceToViewportPoint;
                lookedObj = markedObj;
            }
        }
        
        return lookedObj;
    }
    
    private List<IMarkedByIndicator> CheckMarkedByIndicatorObjs()
    {
        var detectedEnemies = new List<IMarkedByIndicator>();

        RaycastHit[] hits = Physics.SphereCastAll(
            _targetCamera.transform.position,
            0.5f,
            _targetCamera.transform.forward,
            _thresholdToCamera
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out IMarkedByIndicator entity))
            {
                detectedEnemies.Add(entity);
            }
        }
        
        return detectedEnemies;
    }
    
    private bool IsCameraLooking(Vector3 point)
    {
        float distanceToCursor = GetDistanceToCursor(point);
        float distanceToCamera = GetDistanceToCamera(point);
        
        if (_curve != null) return distanceToCursor <= _thresholdToCursor * _curve.Evaluate(distanceToCamera);
        return distanceToCursor <= _thresholdToCamera;
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
    private void OnValidate()
    {
        if (_curve.length < 2) Debug.LogError("Curve has less than 2 keys!");
        _curve.MoveKey(0, new Keyframe(0, _curve[0].value));
        
        if (_curve[_curve.length - 1].time < _thresholdToCamera)
            _curve.MoveKey(_curve.length - 1, new Keyframe(_thresholdToCamera, _curve[_curve.length - 1].value));
        
        if (!_isAutoRedrawCurve) return; 
        if (Mathf.Approximately(_lastThresholdToCamera, _thresholdToCamera))
            return;
        
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