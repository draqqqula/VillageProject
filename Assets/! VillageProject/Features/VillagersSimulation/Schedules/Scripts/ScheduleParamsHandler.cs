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
        if (villager.VillagerData.Profession.Type == ProfessionType.Archer)
        {
            newLoyalty = Mathf.Clamp(newLoyalty, 0.5f, 1f);
        }
        
        var defaultSchedule = _controller.GetDefaultSchedule(villager.VillagerData.Key);
        
        var workPeriods = defaultSchedule.SchedulePeriods.Where(x => x.ActivityType == ActivityType.Work).ToArray();
        if (workPeriods == null || workPeriods.Count() == 0) return;

        var multiplier = _workForLoyaltyCurve.Evaluate(newLoyalty);

        foreach (var period in workPeriods)
        {
            var newLength = (int)Math.Round(period.Length * multiplier, MidpointRounding.AwayFromZero);
            newLength = Mathf.Clamp(newLength, 0, 14);
            _controller.ChangeLengthEvenlyForPeriod(villager.VillagerData.Key, period, newLength);
        }
    }
}