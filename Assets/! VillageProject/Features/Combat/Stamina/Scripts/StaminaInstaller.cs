using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StaminaInstaller : MonoInstaller
{
    [SerializeField] private Stamina _stamina;
    [SerializeField] private Adrenaline _adrenaline;

    public override void InstallBindings()
    {
        Container.BindInstance(_stamina).AsSingle();
        Container.BindInstance(_adrenaline).AsSingle();
    }
}