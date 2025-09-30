using System.Collections;
using UnityEngine;
using Zenject;

public class CharacterHealthInstaller : MonoInstaller
{
    [SerializeField] private Health _health;
    [SerializeField] private DeathEvent _deathEvent;
    public override void InstallBindings()
    {
        Container.BindInstance(_health).AsSingle();
        Container.BindInstance(_deathEvent).AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerHealthSignalInvoker>().AsSingle();
    }
}