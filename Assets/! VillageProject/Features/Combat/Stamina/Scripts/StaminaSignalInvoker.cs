using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;
using R3;
using System;

public class StaminaSignalInvoker : IInitializable
{
    public class StaminaChangedSignal
    {
        public float Value;
    }

    public class AdrenalineChangedSignal
    {
        public float Value;
    }

    public class FatigueSignal
    {
        public bool OnCooldown;
    }

    [Inject] private SignalBus _signalBus;
    [Inject] private Stamina _stamina;
    [Inject] private Adrenaline _adrenaline;

    public void Initialize()
    {
        _stamina.Value.Subscribe(it => _signalBus.Fire(new StaminaChangedSignal() { Value = it/_stamina.MaxValue }));
        _adrenaline.Value.Subscribe(it => _signalBus.Fire(new AdrenalineChangedSignal() { Value = it/_adrenaline.MaxValue }));
        _stamina.IsOnCooldown.Subscribe(it => _signalBus.Fire(new FatigueSignal() { OnCooldown = it }));
    }
}