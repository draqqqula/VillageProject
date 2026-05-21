using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class InterestingPointsService : MonoBehaviour
{
    [SerializeField] private InterestingPoint[] _interestingPoints;

    public InterestingPoint GetRandomInterestingPoint()
    {
        var totalWeight = GetTotalWeight();
        var randomValue = Random.Range(0f, totalWeight);
        float sum = 0;

        foreach (var point in _interestingPoints)
        {
            if (point.IsBusy) continue;
            
            var weight = point.Weight;
            sum += weight;

            if (randomValue <= sum)
            {
                return point;
            }
        }
        
        return null;
    }
    
    private float GetTotalWeight()
    {
        var totalWeight = 0f;

        foreach (var point in _interestingPoints)
        {
            if (point.IsBusy) continue;
            var weight = point.Weight;
            totalWeight += weight;
        }
        
        return totalWeight;
    }
}

[Serializable]
public class InterestingPoint
{
    [field: SerializeField] public Transform Point { get; private set; }
    [field: SerializeField] public float Weight { get; private set; }
    
    [field: SerializeField] public string AnimationTrigger { get; private set; }
    [field: SerializeField] public bool IsBusy { get; set; }
    
    [field: SerializeField] public bool IsCanTalkOnPoint { get; private set; }
}