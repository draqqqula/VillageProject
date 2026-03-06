using System.Linq;
using UnityEngine;

public class VillagerSystem : MonoBehaviour
{
    [field: SerializeField] public Villager[] Villagers { get; private set; }

    public Villager GetVillager(string villagerKey)
    {
        return Villagers.FirstOrDefault(villager => villager.VillagerData.Key == villagerKey);
    }
}