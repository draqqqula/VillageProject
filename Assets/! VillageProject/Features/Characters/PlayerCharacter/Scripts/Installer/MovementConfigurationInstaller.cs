using UnityEngine;
using Zenject;

public class MovementConfigurationInstaller : MonoInstaller
{
    [SerializeField]
    private MovementConfiguration _configuration;

    public override void InstallBindings()
    {
        Container.BindInstance(_configuration).AsSingle();
    }
}
