using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private FirstPersonController _player;
    
    public override void InstallBindings()
    {
        Container.Bind<FirstPersonController>().FromInstance(_player).AsSingle();
    }
}