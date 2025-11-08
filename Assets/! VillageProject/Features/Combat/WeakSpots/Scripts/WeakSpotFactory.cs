using UnityEngine;
using Zenject;

public class WeakSpotFactory
{
    private WeakSpotsTypesData _spotsTypesDataInstance;

    private IInstantiator _instantiator;
    
    public WeakSpotFactory(IInstantiator instantiator, WeakSpotsTypesData spotsTypesDataInstance)
    {
        _instantiator = instantiator;
        _spotsTypesDataInstance = spotsTypesDataInstance;
    }
    
    public WeakSpot Create(WeakSpotType type, Vector3 position, Quaternion rotation, Transform parent)
    {
        var prefab = _spotsTypesDataInstance.GetWeakSpot(type);
        if (prefab == null) return null;
        return _instantiator.InstantiatePrefabForComponent<WeakSpot>(prefab, position, rotation, parent);
    }
}