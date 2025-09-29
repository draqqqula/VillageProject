using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;
using R3;

public class StaminaSignalInvoker : MonoBehaviour
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

    [Inject(Source = InjectSources.Parent)] private SignalBus _signalBus;
    [Inject] private Stamina _stamina;
    [Inject] private Adrenaline _adrenaline;

    public void Awake()
    {
        _signalBus.DeclareSignal<StaminaChangedSignal>();
        _signalBus.DeclareSignal<AdrenalineChangedSignal>();
        _signalBus.DeclareSignal<FatigueSignal>();
        _stamina.Value.Subscribe(it => _signalBus.Fire(new StaminaChangedSignal() { Value = it/_stamina.MaxValue })).AddTo(this);
        _adrenaline.Value.Subscribe(it => _signalBus.Fire(new AdrenalineChangedSignal() { Value = it/_adrenaline.MaxValue })).AddTo(this);
        _stamina.IsOnCooldown.Subscribe(it => Debug.Log("!!")).AddTo(this);
    }
}