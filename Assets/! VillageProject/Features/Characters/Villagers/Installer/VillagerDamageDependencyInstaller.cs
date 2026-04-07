using UnityEngine;
using Zenject;

public class VillagerDamageDependencyInstaller : MonoInstaller
{
    [SerializeField] private TeamMember _teamMember;
    [SerializeField] private Villager _villager;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_teamMember).AsSingle();
        Container.BindInstance(_villager).AsSingle();
    }
}