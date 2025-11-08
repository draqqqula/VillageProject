using System.Collections;
using UnityEngine;
using Zenject;

public class EnemyDamageDependencyInstaller : MonoInstaller
{
    [SerializeField] private DamageTilt _damageTilt;
    [SerializeField] private WeakSpotController _weakSpotController;
    [SerializeField] private TeamMember _teamMember;

    public override void InstallBindings()
    {
        Container.BindInstance(_damageTilt).AsSingle();
        Container.BindInstance(_weakSpotController).AsSingle();
        Container.BindInstance(_teamMember).AsSingle();
    }
}