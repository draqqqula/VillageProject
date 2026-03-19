using System;
using System.Linq;
using UnityEngine;

public class VillagerSystem : MonoBehaviour
{
    [field: SerializeField] public Villager[] Villagers { get; private set; }
    [SerializeField] private HomeService _homeService;
    [SerializeField] private Transform _villageCenter;

    private void Awake()
    {
        foreach (var villager in Villagers)
        {
            villager.Init(_homeService, _villageCenter);
        }
    }

    public Villager GetVillager(string villagerKey)
    {
        return Villagers.FirstOrDefault(villager => villager.VillagerData.Key == villagerKey);
    }
}