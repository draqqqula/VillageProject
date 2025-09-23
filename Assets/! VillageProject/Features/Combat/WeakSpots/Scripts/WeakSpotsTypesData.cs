using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Weak Spot Types Data", menuName = "Combat/New Weak Spot Types")]
public class WeakSpotsTypesData : ScriptableObject
{
    [field: SerializeField] public WeakSpotInfo[] WeakSpots { get; private set; }

    public WeakSpot GetWeakSpot(WeakSpotType type)
    {
        var info = WeakSpots.FirstOrDefault(info => info.Type == type);
        if (info == null)
        {
            Debug.LogError($"Weak spot with type {type} does not exist!");
            return null;
        }
        return info.Prefab;
    }
    
    [Serializable]
    public class WeakSpotInfo
    {
        [field: SerializeField] public WeakSpotType Type { get; private set; }
        [field: SerializeField] public WeakSpot Prefab { get; private set; }
    }
}