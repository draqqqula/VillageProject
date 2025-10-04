using UnityEngine;

public sealed class IndicatorActivator
{
    private GameObject _indicator;
    private Camera _targetCamera;

    private IMarkedByIndicator _currentMarkedObj;
    
    public IndicatorActivator(GameObject indicatorObject, Camera camera)
    {
        _indicator = indicatorObject;
        _indicator.SetActive(false);
        
        _targetCamera = camera;
    }
    
    public void ActivateIndicator(IMarkedByIndicator entity)
    {
        if (entity == _currentMarkedObj) return;
        _currentMarkedObj = entity;
        _currentMarkedObj.OnDestroyed += DeactivateIndicator;
        
        var position = _targetCamera.WorldToScreenPoint(entity.OriginPoint.position);
        _indicator.transform.position = position;
        _indicator.SetActive(true);
    }

    public void UpdateIndicator()
    {
        if (_currentMarkedObj == null) return;
        var position = _targetCamera.WorldToScreenPoint(_currentMarkedObj.OriginPoint.position);
        _indicator.transform.position = position;
    }

    public void DeactivateIndicator()
    {
        if (_currentMarkedObj == null) return;
        
        _currentMarkedObj.OnDestroyed -= DeactivateIndicator;
        _currentMarkedObj = null;

        _indicator.transform.position = Vector3.zero;
        _indicator.SetActive(false);
    }
}
