using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

public class WeakSpotController : MonoBehaviour
{
    [SerializeField] private EnemyWeakSpotsData _enemySpotsData;
    private EnemyWeakSpotsData _enemyWeakSpotsDataInstance;
    private WeakSpotsTypesData _spotsTypesDataInstance;
    
    [FormerlySerializedAs("_bodyParts1")] [SerializeField] private List<GameObject> _bodyParts;
    private Collider _collider;

    private WeakSpotFactory _weakSpotFactory;
    private CommonRandomizer _randomizer;

    [Inject]
    private void Construct(IInstantiator instantiator)
    {
        _enemyWeakSpotsDataInstance = ScriptableObject.Instantiate(_enemySpotsData);
        _spotsTypesDataInstance = ScriptableObject.Instantiate(_enemySpotsData.WeakSpotsTypesData);
        _weakSpotFactory = new WeakSpotFactory(instantiator, _spotsTypesDataInstance);
        
        _randomizer = new CommonRandomizer();
    }
    
    private void Reset()
    {
        GetComponentsInChildren(_bodyParts);
    }

    public bool IsOpened => _collider != null;

    public void Open()
    {
        if (IsOpened)
        {
            return;
        }

        var weakSpotType = _randomizer.RandomValueWithProbability(_enemyWeakSpotsDataInstance.ProbabilityInfos).Type;
        var spot = _randomizer.RandomValue(_bodyParts.ToArray());
        var effect = _weakSpotFactory.Create(weakSpotType, spot.transform.position, Quaternion.identity, spot.transform);
        _collider = effect?.GetComponent<Collider>();
    }
    
    public void Close()
    {
        if (!IsOpened)
        {
            return;
        }
        Destroy(_collider.gameObject);
        _collider = null;
    }

    public bool Raycast(Ray ray, float maxDistance)
    {
        if (!IsOpened)
        {
            return false;
        }
        return _collider.Raycast(ray, out var hitInfo, maxDistance);
    }
}