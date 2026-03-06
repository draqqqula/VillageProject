using UnityEngine;

[CreateAssetMenu(fileName = "VillagerData", menuName = "Villagers Simulation/New Villager Data")]
public class VillagerData : ScriptableObject
{
    [field: SerializeField] public string Key {get; private set;}
    [field: SerializeField] public ActivityType ActivityType {get; set;}
}