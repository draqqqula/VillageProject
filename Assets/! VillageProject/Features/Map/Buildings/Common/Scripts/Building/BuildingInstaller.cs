using System.Collections;
using UnityEngine;
using Zenject;

public class BuildingInstaller : MonoInstaller
{
    [SerializeField] private BuildingMenuDisplay _display;
    [SerializeField] private BuildingPlanner _buildingPlanner;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_display).AsSingle();
        Container.Bind<BuildingStorage>().FromNew().AsSingle();
        Container.Bind<BuildingPlanner>().FromInstance(_buildingPlanner).AsSingle();
    }
}