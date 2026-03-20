using UnityEngine;
using Zenject;

public class VillagerDamageDependencyInstaller : MonoInstaller
{
    [SerializeField] private TeamMember _teamMember;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_teamMember).AsSingle();
    }
}