using R3;
using TMPro;
using UnityEngine;
using Zenject;

public class SimulationDataView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _loyaltyText;
    [SerializeField] private TextMeshProUGUI _experienceText;
    [SerializeField] private TextMeshProUGUI _healthText;
    
    private VillagerSystem _villagerSystem;

    [Inject]
    private void Construct(VillagerSystem villagerSystem)
    {
        _villagerSystem = villagerSystem;
    }

    public void Init()
    {
        foreach (var villager in _villagerSystem.Villagers)
        {
            villager.VillagerData.Loyalty.Property.Subscribe(OnValueChanged).AddTo(this);
            villager.VillagerData.Profession.Experience.Subscribe(OnValueChanged).AddTo(this);
            villager.VillagerData.Health.AmountReactive.Subscribe(OnValueChanged).AddTo(this);
        }
    }

    private void OnValueChanged(float value)
    {
        CalculateAverageValues(out float loyalty, out float experience, out float health);

        _loyaltyText.text = loyalty.ToString("F2");
        _experienceText.text = experience.ToString("F2");
        _healthText.text = health.ToString("F0");
    }

    private void CalculateAverageValues(out float averageLoyalty, out float averageExperience, out float averageHealth)
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
        
        averageLoyalty = sumLoyalty / _villagerSystem.Villagers.Length;
        averageExperience = sumExperience / _villagerSystem.Villagers.Length;
        averageHealth = sumHealth / _villagerSystem.Villagers.Length;
    }
}