using System;
using UnityEngine;
using Zenject;

[Serializable]
public abstract class Upgrade
{
    public abstract void Apply();
}

[Serializable]
public class SwordUpgrade : Upgrade
{
    [field: SerializeField] public float SwordMultiplier {get; private set;}
    [Inject] private AttackBonus _attackBonus;
    
    public override void Apply()
    {
        _attackBonus.DamageMultiplier = SwordMultiplier;
    }
}

[Serializable]
public class ArmorUpgrade : Upgrade
{
    [field: SerializeField] public float MaxHealth {get; private set;}
    [Inject] private FirstPersonController _firstPersonController;
    
    public override void Apply()
    {
        var health = _firstPersonController.gameObject.GetComponent<Health>();
        health.MaxHealth = MaxHealth;
        health.Amount = MaxHealth;
    }
}