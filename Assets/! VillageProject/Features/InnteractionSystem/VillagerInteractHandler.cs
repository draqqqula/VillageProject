using System;
using UnityEngine;
using Zenject;

public class VillagerInteractHandler : IDisposable
{
    private InteractTrigger _interactTrigger;
    private Villager _villager;

    private UpgradeMenu _armorUpgrader;
    private UpgradeMenu _swordUpgrader;

    public VillagerInteractHandler(Villager villager, InteractTrigger interactTrigger, DiContainer container)
    {
        _interactTrigger = interactTrigger;

        _villager = villager;
        OnProfessionChanged(villager.VillagerData.Profession);
        _villager.OnProfessionChanged += OnProfessionChanged;
        
        _armorUpgrader = container.ResolveId<UpgradeMenu>("UpgradeArmor");
        _swordUpgrader = container.ResolveId<UpgradeMenu>("UpgradeSword");
    }

    private void OnProfessionChanged(Profession profession)
    {
        if (profession.Type == ProfessionType.Blacksmith || profession.Type == ProfessionType.Armorer)
            _interactTrigger.gameObject.SetActive(true);
        else
            _interactTrigger.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (_villager.VillagerData.Profession.Type == ProfessionType.Blacksmith)
            _swordUpgrader.Activate(_villager);
        else if (_villager.VillagerData.Profession.Type == ProfessionType.Armorer)
            _armorUpgrader.Activate(_villager);
    }

    public void Dispose()
    {
        _villager.OnProfessionChanged -= OnProfessionChanged;
    }
}