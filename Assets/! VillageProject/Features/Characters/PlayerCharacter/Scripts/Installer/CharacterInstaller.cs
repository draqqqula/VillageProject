using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller
{
    [SerializeField] private CharacterVelocity _characterVelocity;
    [SerializeField] private TeamMember _teamMember;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private HorizontalMovement _horizontalMovement;
    [SerializeField] private FirstPersonController _firstPersonController;
    
    public override void InstallBindings()
    {
        Container.Bind<CharacterVelocity>().FromInstance(_characterVelocity).AsSingle();
        Container.BindInterfacesAndSelfTo<FirstPersonController>().FromInstance(_firstPersonController).AsSingle();
        Container.BindInstance(_teamMember).AsSingle();
        Container.BindInstance(_horizontalMovement).AsSingle();
        Container.BindInstance(_raycastOrigin).WithId("Raycast").AsCached();
        Container.BindInterfacesAndSelfTo<PlayerHealthSignalInvoker>().AsSingle();
    }

    private void Reset()
    {
        _firstPersonController = GetComponent<FirstPersonController>();
        _teamMember = GetComponent<TeamMember>();
        _characterVelocity = GetComponent<CharacterVelocity>();
    }
}
