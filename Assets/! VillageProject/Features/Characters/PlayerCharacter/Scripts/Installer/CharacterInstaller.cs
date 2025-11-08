using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller
{
    [SerializeField] private CharacterVelocity _characterVelocity;
    [SerializeField] private TeamMember _teamMember;
    [SerializeField] private Transform _raycastOrigin;
    
    public override void InstallBindings()
    {
        Container.Bind<CharacterVelocity>().FromInstance(_characterVelocity).AsSingle();
        Container.BindInstance(_teamMember).AsSingle();
        Container.BindInstance(_raycastOrigin).WithId("Raycast").AsCached();
        Container.BindInterfacesAndSelfTo<PlayerHealthSignalInvoker>().AsSingle();
    }
}
