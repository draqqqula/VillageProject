using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class MeleeControlsPresetInstaller : MonoInstaller
{
    [SerializeField] private int _presetCount;
    [SerializeField] private InputActionReference _inputAction;

    public override void InstallBindings()
    {
        
        Container.BindInstance(new MeleeControlsPresetManager(_presetCount, _inputAction)).AsSingle();
    }
}