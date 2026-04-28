using UnityEngine;
using Zenject;

public class ArrowDamageInstaller : MonoInstaller<ArrowDamageInstaller>
{
    private AttackBonus _attackBonus;
    public AttackBonus AttackBonus => _attackBonus;
    
    public override void InstallBindings()
    {
        _attackBonus = new AttackBonus();
        Container.BindInstance(_attackBonus).AsSingle();
    }
}
