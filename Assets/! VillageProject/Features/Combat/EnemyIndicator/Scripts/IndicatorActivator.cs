using UnityEngine;

public sealed class IndicatorActivator
{
    private GameObject _indicator;
    private Camera _targetCamera;

    public Origin LockedOrigin {get; private set;}
    
    public IndicatorActivator(GameObject indicatorObject, Camera camera)
    {
        _indicator = indicatorObject;
        _indicator.SetActive(false);
        
        _targetCamera = camera;
    }
    
    public void ActivateIndicator(Origin origin)
    {
        if (origin == LockedOrigin) return;
        
        LockedOrigin = origin;
        LockedOrigin.OnDestroyed += DeactivateIndicator;
        
        var position = _targetCamera.WorldToScreenPoint(origin.OriginPoint.position);
        if (position.z < 0) return;
        
        _indicator.transform.position = position;
        _indicator.SetActive(true);
    }

    public void UpdateIndicator()
    {
        if (LockedOrigin == null) return;
        
        var position = _targetCamera.WorldToScreenPoint(LockedOrigin.OriginPoint.position);
        if (position.z < 0) return;
        
        _indicator.transform.position = position;
    }

    public void DeactivateIndicator()
    {
        if (LockedOrigin == null) return;
        
        LockedOrigin.OnDestroyed -= DeactivateIndicator;
        LockedOrigin = null;
        
        _indicator.transform.position = Vector3.zero;
        _indicator.SetActive(false);
    }
}
