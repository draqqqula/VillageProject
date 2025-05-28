using System.Collections;
using UnityEngine;
using Zenject;

public class CompassInstaller : MonoInstaller
{
    [SerializeField] private CompassMarkerManager _markerManager;
    [SerializeField] private CompassOrigin _origin;
    public override void InstallBindings()
    {
        Container.BindInstance(_markerManager).AsSingle();
        Container.BindInstance(_origin).AsSingle();
    }
}