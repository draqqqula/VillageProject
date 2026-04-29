using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private FirstPersonController _player;
    [SerializeField] private InteractTip _interactTip;
    
    public override void InstallBindings()
    {
        Container.Bind<FirstPersonController>().FromInstance(_player).AsSingle();
        Container.Bind<InteractTip>().FromInstance(_interactTip).AsSingle();
    }
}