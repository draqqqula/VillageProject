using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class WeakSpotController : MonoBehaviour
{
    [SerializeField] private EnemyWeakSpotsData _enemySpotsData;
    private EnemyWeakSpotsData _enemyWeakSpotsDataInstance;
    private WeakSpotsTypesData _spotsTypesDataInstance;
    
    [SerializeField] private List<BodyPart> _bodyPartsList;
    private Collider _collider;

    private WeakSpotFactory _weakSpotFactory;
    private CommonRandomizer _randomizer;

    public bool _onlyOneWeakSpotType = false;
    public WeakSpotType _oneWeakSpotType = WeakSpotType.Thrust;

    [Serializable]
    private class BodyPart
    {
        [field: SerializeField] public GameObject BodyObject { get; set; }
        [field: SerializeField] public WeakSpotType[] ConnectedWeakSpot { get; set; }
    }
    
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
        GetComponentsInChildren(_bodyPartsList);
    }

    private ReactiveProperty<bool> _isOpened = new ReactiveProperty<bool>();
    public ReadOnlyReactiveProperty<bool> IsOpened => _isOpened;

    public void Open()
    {
        if (IsOpened.CurrentValue)
        {
            return;
        }

        WeakSpotType weakSpotType;
        if (_onlyOneWeakSpotType)
        {
            weakSpotType = _oneWeakSpotType;
        }
        else
        {
            weakSpotType = _randomizer.RandomValueWithProbability(_enemyWeakSpotsDataInstance.ProbabilityInfos).Type;
        }
        var spot = _randomizer.RandomValue(_bodyPartsList
            .Where(bodyPart => bodyPart.ConnectedWeakSpot.Contains(weakSpotType))
            .ToArray())?.BodyObject;

        
        if (spot == null) return;
        var effect = _weakSpotFactory.Create(weakSpotType, spot.transform.position, Quaternion.identity, spot.transform);
        _collider = effect?.GetComponent<Collider>();
        _isOpened.Value = true;
    }
    
    public void Close()
    {
        if (!IsOpened.CurrentValue)
        {
            return;
        }
        Destroy(_collider.gameObject);
        _collider = null;
        _isOpened.Value = false;
    }

    public bool Raycast(Ray ray, float maxDistance)
    {
        if (!IsOpened.CurrentValue)
        {
            return false;
        }
        return _collider.Raycast(ray, out var hitInfo, maxDistance);
    }
}


