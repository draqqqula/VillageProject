using R3;
using UnityEngine;
using Zenject;

public class LoyaltyController : MonoBehaviour
{
    [SerializeField] private AnimationCurve _increaseByWaveCurve;
    [SerializeField] private AnimationCurve _decreaseByWaveCurve;
    
    [SerializeField] private AnimationCurve _decreaseHealthForLoyaltyCurve;
    
    [Inject] private MatchObjective _matchObjective;
    [Inject] private WaveController _waveController;
    [Inject] private VillagerSystem _villagerSystem;
    
    private bool _isEnemiesInVillage = false;
    
    public void Init()
    {
        _matchObjective.OnEnemiesInVillage += OnEnemiesInVillage;
        _waveController.OnWaveCompleted += OnWaveCompleted;

        foreach (var villager in _villagerSystem.Villagers)
        {
            villager.VillagerData.Health.AmountReactive.Subscribe(value => OnHealthChanged(villager, value)).AddTo(villager.gameObject);
        }
    }

    private void OnEnemiesInVillage()
    {
        _isEnemiesInVillage = true;

        var decreasePercentage = _decreaseByWaveCurve.Evaluate(_waveController.CurrentWave.Difficulty);
        DecreaseLoyalty(decreasePercentage);
    }

    private void OnWaveCompleted(WaveInfo waveInfo)
    {
        if (!_isEnemiesInVillage)
        {
            var increasePercentage = _increaseByWaveCurve.Evaluate(waveInfo.Difficulty);
            IncreaseLoyalty(increasePercentage);
        }
        _isEnemiesInVillage = false;
    }

    private void OnHealthChanged(Villager villager, float health)
    {
        var normalizedHealth = health / villager.VillagerData.Health.MaxHealth;

        var decreaseByHealthPercentage= _decreaseHealthForLoyaltyCurve.Evaluate(normalizedHealth);
        var newLoyalty = Mathf.Clamp01(villager.VillagerData.Loyalty.Property.Value + villager.VillagerData.Loyalty.DecreaseByHealthPercentage);
        newLoyalty = Mathf.Clamp01(newLoyalty - decreaseByHealthPercentage);
        
        villager.VillagerData.Loyalty.DecreaseByHealthPercentage = decreaseByHealthPercentage;
        villager.VillagerData.Loyalty.Property.Value = newLoyalty;
    }

    public void IncreaseLoyalty(float percentage)
    {
        ChangeLoyalty(percentage);
    }

    public void DecreaseLoyalty(float percentage)
    {
        ChangeLoyalty(-percentage);
    }

    private void ChangeLoyalty(float percentage)
    {
        foreach (var villager in _villagerSystem.Villagers)
        {
            villager.VillagerData.Loyalty.Property.Value = Mathf.Clamp01(villager.VillagerData.Loyalty.Property.Value + percentage);
        }
    }

    private void OnDestroy()
    {
        _matchObjective.OnEnemiesInVillage -= OnEnemiesInVillage;
        _waveController.OnWaveCompleted -= OnWaveCompleted;
    }
}