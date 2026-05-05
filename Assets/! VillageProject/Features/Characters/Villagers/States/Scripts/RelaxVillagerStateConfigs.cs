using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RelaxStatesConfig", menuName = "Villagers Simulation/New Relax States Config")]
public class RelaxVillagerStateConfigs : ScriptableObject
{
    [field:SerializeField] public List<ChoosingStateConfig>StatesConfigs { get; private set; }
}

[Serializable]
public class ChoosingStateConfig
{
    [field: SerializeField] public string StateName { get; private set; }
    
    [field: SerializeField] public AnimationCurve ActivityHoursByLoyalty { get; private set; }
    [field: SerializeField] public AnimationCurve WeightByLoyalty {get; private set;}
    
    [field: SerializeField] public int ActivityHours { get; private set; }
    [field: SerializeField, Range(0f, 1f)] public float Weight {get; private set;}
    
    public RelaxVillagerState VillagerState {get; set;}
    
}