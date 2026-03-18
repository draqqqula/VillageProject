using System;
using System.Linq;
using UnityEngine;

public class VillagerSystem : MonoBehaviour
{
    [field: SerializeField] public Villager[] Villagers { get; private set; }
    [SerializeField] private ProfessionService _professionService;
    [SerializeField] private HomeService _homeService;

    private void Awake()
    {
        foreach (var villager in Villagers)
        {
            villager.Init(_professionService, _homeService);
        }
    }

    public Villager GetVillager(string villagerKey)
    {
        return Villagers.FirstOrDefault(villager => villager.VillagerData.Key == villagerKey);
    }
}