using System.Collections;
using UnityEngine;
using Zenject;

public class BuildingInstaller : MonoInstaller
{
    [SerializeField] private BuildingMenuDisplay _display;
    public override void InstallBindings()
    {
        Container.BindInstance(_display).AsSingle();
    }
}