using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StaminaInstaller : MonoInstaller
{
    [SerializeField] private Stamina _stamina;
    [SerializeField] private Adrenaline _adrenaline;
    [SerializeField] private AdrenalineToStaminaRate _adrenalineToStaminaRate;

    public override void InstallBindings()
    {
        Container.BindInstance(_stamina).AsSingle();
        Container.BindInstance(_adrenaline).AsSingle();
        Container.BindInstance(_adrenalineToStaminaRate).AsSingle();
        Container.BindInterfacesAndSelfTo<StaminaSignalInvoker>().AsSingle();
    }
}