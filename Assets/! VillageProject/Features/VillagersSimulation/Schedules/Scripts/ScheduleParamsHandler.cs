using System;
using System.Linq;
using R3;
using UnityEngine;

public class ScheduleParamsHandler : MonoBehaviour
{
    private ScheduleController _controller;
    private VillagerSystem _villagerSystem;
    
    [SerializeField] private AnimationCurve _workForLoyaltyCurve;
    
    public void Init(ScheduleController scheduleController, VillagerSystem villagerSystem)
    {
        _controller = scheduleController;
        _villagerSystem = villagerSystem;

        foreach (var villager in villagerSystem.Villagers)
        {
            villager.VillagerData.Loyalty.Property.Subscribe((value) => OnLoyaltyChanged(villager, value)).AddTo(villager.gameObject);
        }
    }
    
    private void OnLoyaltyChanged(Villager villager, float newLoyalty)
    {
        var defaultSchedule = _controller.GetDefaultSchedule(villager.VillagerData.Key);
        
        var workPeriod = defaultSchedule.SchedulePeriods.FirstOrDefault(x => x.ActivityType == ActivityType.Work);

        var multiplier = _workForLoyaltyCurve.Evaluate(newLoyalty);
        var newLength = (int)Math.Round(workPeriod.Length * multiplier, MidpointRounding.AwayFromZero);
        _controller.ChangeLengthEvenlyForPeriod(villager.VillagerData.Key, workPeriod, newLength);
    }
}