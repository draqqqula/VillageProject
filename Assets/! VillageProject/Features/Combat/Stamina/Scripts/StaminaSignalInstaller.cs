using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class StaminaSignalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<StaminaSignalInvoker.StaminaChangedSignal>();
        Container.DeclareSignal<StaminaSignalInvoker.AdrenalineChangedSignal>();
        Container.DeclareSignal<StaminaSignalInvoker.FatigueSignal>();
        Container.DeclareSignal<StaminaSignalInvoker.StaminaHoverSignal>();
    }
}