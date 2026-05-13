using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class VillagerSimulationSettings : MonoBehaviour
{
    [SerializeField] private Slider _loyalty;
    [SerializeField] private Slider _experience;
    [SerializeField] private Slider _health;
    
    [Inject] private VillagerSystem _villagerSystem;
    
    private void OnEnable()
    {
        var sumLoyalty = 0f;
        var sumExperience = 0f;
        var sumHealth = 0f;
        
        foreach (var villager in _villagerSystem.Villagers)
        {
            sumLoyalty += villager.VillagerData.Loyalty.Property.CurrentValue;
            sumExperience += villager.VillagerData.Profession.Experience.CurrentValue;
            sumHealth += villager.VillagerData.Health.Amount;
        }

        _loyalty.value = sumLoyalty / _villagerSystem.Villagers.Length;
        _loyalty.onValueChanged.AddListener(OnLoyaltyChanged);

        _experience.value = sumExperience / _villagerSystem.Villagers.Length;
        _experience.onValueChanged.AddListener(OnExperienceChanged);

        _health.value = sumHealth / _villagerSystem.Villagers.Length;
        _health.onValueChanged.AddListener(OnHealthChanged);
    }

    private void OnDisable()
    {
        _loyalty.onValueChanged.RemoveAllListeners();
        _experience.onValueChanged.RemoveAllListeners();
        _health.onValueChanged.RemoveAllListeners();
    }
    
    private void OnLoyaltyChanged(float value)
    {
        foreach (var villager in _villagerSystem.Villagers)
        {
            villager.VillagerData.Loyalty.Property.Value = value;
        }
    }

    private void OnExperienceChanged(float value)
    {
        foreach (var villager in _villagerSystem.Villagers)
        {
            villager.VillagerData.Profession.Experience.Value = value;
        }
    }

    private void OnHealthChanged(float value)
    {
        foreach (var villager in _villagerSystem.Villagers)
        {
            villager.VillagerData.Health.Amount = value;
        }
    }
}