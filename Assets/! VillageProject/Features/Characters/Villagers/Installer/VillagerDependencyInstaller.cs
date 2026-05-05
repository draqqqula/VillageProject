using UnityEngine;
using Zenject;

public class VillagerDependencyInstaller : MonoInstaller
{
    [SerializeField] private TeamMember _teamMember;
    [SerializeField] private Villager _villager;
    [SerializeField] private Collider _discoveryCollider;
    [SerializeField] private SearchForTarget _searchForTarget;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_teamMember).AsSingle();
        Container.BindInstance(_villager).AsSingle();
        Container.BindInstance(_discoveryCollider).WithId("Discovery").AsSingle();
        Container.BindInstance(_searchForTarget).AsSingle();
        
        Container.Bind<AttackBonus>().FromNew().AsSingle();
    }
}