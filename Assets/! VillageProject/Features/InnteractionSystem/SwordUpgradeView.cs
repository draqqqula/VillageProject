using System;
using Zenject;

public sealed class SwordUpgradeView : UpgradeView
{
    private AttackBonus _attackBonus;

    [Inject]
    private void Construct(AttackBonus attackBonus)
    {
        _attackBonus = attackBonus;
    }
    
    public override void UpdateView(UpgradeConfig config)
    {
        var upgrade = config.Upgrade as SwordUpgrade;
        var currentUpgrade = _attackBonus.DamageMultiplier;
        var upgradePercentage = ((upgrade.SwordMultiplier - currentUpgrade) / currentUpgrade) * 100;
        
        _upgradeTitle.text = $"Меч {config.Level}";
        _upgradeDescription.text = $"Атака +{upgradePercentage:0.#}%";
        base.UpdateView(config);
    }
}