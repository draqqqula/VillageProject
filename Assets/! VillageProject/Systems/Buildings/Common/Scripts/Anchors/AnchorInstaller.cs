using System.Collections;
using UnityEngine;
using Zenject;

public class AnchorInstaller : MonoInstaller
{
    [SerializeField] private AnchorMode _anchorMode;
    public override void InstallBindings()
    {
        Container.BindInstance(_anchorMode).AsSingle();
    }
}