using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VillagerSystem : MonoBehaviour
{
    [field: SerializeField] public Villager[] Villagers { get; private set; }
    [SerializeField] private HomeService _homeService;
    [SerializeField] private Transform _villageCenter;
    [SerializeField] private GameTimer _gameTimer;
    
    public event Action<Villager> OnAddedVillager;

    public void Init()
    {
        foreach (var villager in Villagers)
        {
            villager.Init(_homeService);
        }
    }

    public Villager GetVillager(string villagerKey)
    {
        return Villagers.FirstOrDefault(villager => villager.VillagerData.Key == villagerKey);
    }

    public Villager GetVillager(ActivityType[] activities)
    {
        var villagers = Villagers.Where(villager => activities.Contains(villager.VillagerData.ActivityType.Value)).ToArray();
        return villagers[Random.Range(0, villagers.Length)];
    }

    public void AddVillager(Villager villager)
    {
        OnAddedVillager?.Invoke(villager);
    }
}