using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Weak Spots Data", menuName = "Combat/New Enemy Weak Spot")]
public class EnemyWeakSpotsData : ScriptableObject
{
    [field: SerializeField] public WeakSpotProbabilityInfo[] ProbabilityInfos { get; private set; }
    [SerializeField] private bool _autoProbabilityFix = true;
    [field: SerializeField] public WeakSpotsTypesData WeakSpotsTypesData { get; private set; }

    [Serializable]
    public class WeakSpotProbabilityInfo : IRandomizableElement
    {
        [field: SerializeField] public WeakSpotType Type { get; private set; }
        [field: SerializeField, Range(0, 1)] public float Probability { get; set; }
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!_autoProbabilityFix) return;
        
        float sum = 0;
        for (int i = 0; i < ProbabilityInfos.Length; i++)
        {
            if (sum + ProbabilityInfos[i].Probability > 1) 
                ProbabilityInfos[i].Probability = Mathf.Clamp(ProbabilityInfos[i].Probability, 0, 1 - sum);
            sum += ProbabilityInfos[i].Probability;
        }
        
        EditorUtility.SetDirty(this);
    }
#endif
}