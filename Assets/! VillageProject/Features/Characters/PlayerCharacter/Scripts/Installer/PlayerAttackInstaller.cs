using UnityEngine;
using Zenject;

public class PlayerAttackInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AttackBonus>().FromNew().AsSingle();
    }
}