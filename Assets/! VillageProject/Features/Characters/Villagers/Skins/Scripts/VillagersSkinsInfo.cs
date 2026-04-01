using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Villager Skin Info", menuName = "Villagers Simulation/New Skin Info")]
public class VillagersSkinsInfo : ScriptableObject
{
    [field: SerializeField] public SkinConfig[] SkinConfigs { get; private set; }
}

[Serializable]
public class SkinConfig
{
    [field: SerializeField] public Gender Gender {get; private set;}
    [field: SerializeField] public ProfessionType Profession {get; private set;}
    [field: SerializeField] public GameObject Prefab {get; private set;}
}