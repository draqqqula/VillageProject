using UnityEngine;

public sealed class IndicatorActivator
{
    private GameObject _indicator;

    private IMarkedByIndicator _currentMarkedObj;
    
    public IndicatorActivator(GameObject indicatorPrefab)
    {
        _indicator = GameObject.Instantiate(indicatorPrefab, Vector3.zero, Quaternion.identity);
        _indicator.SetActive(false);
    }
    
    public void ActivateIndicator(IMarkedByIndicator entity)
    {
        if (entity == _currentMarkedObj) return;
        _currentMarkedObj = entity;
        
        _indicator.transform.SetParent(entity.OriginPoint);
        _indicator.transform.localPosition = Vector3.zero;
        _indicator.transform.localRotation = Quaternion.identity;
        
        _indicator.SetActive(true);
    }

    public void DeactivateIndicator()
    {
        _currentMarkedObj = null;
        _indicator.transform.SetParent(null);
        _indicator.SetActive(false);
    }
}
