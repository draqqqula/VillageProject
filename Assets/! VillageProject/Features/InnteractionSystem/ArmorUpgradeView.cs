using Zenject;

public sealed class ArmorUpgradeView : UpgradeView
{
    private Health _health;

    [Inject]
    private void Construct(FirstPersonController firstPersonController)
    {
        _health = firstPersonController.gameObject.GetComponent<Health>();
    }
    
    public override void UpdateView(UpgradeConfig config)
    {
        var upgrade = config.Upgrade as ArmorUpgrade;
        var currentUpgrade = _health.MaxHealth;
        var upgradePercentage = ((upgrade.MaxHealth - currentUpgrade) / currentUpgrade) * 100;

        _upgradeTitle.text = $"Броня {config.Level}";
        _upgradeDescription.text = $"Здоровье +{upgradePercentage:0.#}%";
        base.UpdateView(config);
    }
}