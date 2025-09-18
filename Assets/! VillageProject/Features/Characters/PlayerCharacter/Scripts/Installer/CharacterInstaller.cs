using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller
{
    [SerializeField] private CharacterVelocity _characterVelocity;
    
    public override void InstallBindings()
    {
        Container.Bind<CharacterVelocity>().FromInstance(_characterVelocity).AsSingle();
    }
}
