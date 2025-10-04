using UnityEngine;

public sealed class IndicatorActivator
{
    private GameObject _indicator;
    private Camera _targetCamera;

    private Origin _currentOrigin;
    
    public IndicatorActivator(GameObject indicatorObject, Camera camera)
    {
        _indicator = indicatorObject;
        _indicator.SetActive(false);
        
        _targetCamera = camera;
    }
    
    public void ActivateIndicator(Origin origin)
    {
        if (origin == _currentOrigin) return;
        _currentOrigin = origin;
        _currentOrigin.OnDestroyed += DeactivateIndicator;
        
        var position = _targetCamera.WorldToScreenPoint(origin.OriginPoint.position);
        _indicator.transform.position = position;
        _indicator.SetActive(true);
    }

    public void UpdateIndicator()
    {
        if (_currentOrigin == null) return;
        var position = _targetCamera.WorldToScreenPoint(_currentOrigin.OriginPoint.position);
        _indicator.transform.position = position;
    }

    public void DeactivateIndicator()
    {
        if (_currentOrigin == null) return;
        
        _currentOrigin.OnDestroyed -= DeactivateIndicator;
        _currentOrigin = null;

        _indicator.transform.position = Vector3.zero;
        _indicator.SetActive(false);
    }
}
