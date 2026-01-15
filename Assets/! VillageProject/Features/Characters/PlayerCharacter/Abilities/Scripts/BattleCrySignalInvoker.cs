using System;
using UnityEngine;
using Zenject;

public class BattleCrySignalInvoker : IInitializable, IDisposable
{
    private SignalBus _signalBus;
    private BattleCry _battleCry;
    
    [Inject]
    private void Construct(SignalBus signalBus, BattleCry battleCry)
    {
        _signalBus = signalBus;
        _battleCry = battleCry;
    }

    public class BattleCryStartedSignal
    {
        
    }
    
    public class BattleCryCooldownProgressSignal
    {
        public float Progress;
    }

    public class BattleCryFinishedSignal
    {
        
    }
    
    public class BattleCryStartCooldownSignal
    {

    }

    public class BattleCryCooldownFinishedSignal
    {
        
    }

    public void Initialize()
    {
        _battleCry.OnStarted += OnStartedDelegate;
        _battleCry.OnFinished += OnFinishedDelegate;
        _battleCry.OnCooldownStarted += OnCooldownStartedDelegate;
        _battleCry.OnCooldownProgress += OnCooldownProgressDelegate;
        _battleCry.OnFinishedCooldown += OnCooldownDelegate;
    }

    public void Dispose()
    {
        _battleCry.OnStarted -= OnStartedDelegate;
        _battleCry.OnFinished -= OnFinishedDelegate;
        _battleCry.OnCooldownStarted -= OnCooldownStartedDelegate;
        _battleCry.OnCooldownProgress -= OnCooldownProgressDelegate;
        _battleCry.OnFinishedCooldown -= OnCooldownDelegate;
    }
    
    private void OnStartedDelegate() => _signalBus.Fire(new BattleCryStartedSignal());
    private void OnFinishedDelegate() => _signalBus.Fire(new BattleCryFinishedSignal());
    private void OnCooldownStartedDelegate() => _signalBus.Fire(new BattleCryStartCooldownSignal() {});
    private void OnCooldownProgressDelegate(float value) => _signalBus.Fire(new BattleCryCooldownProgressSignal() {Progress = value});
    private void OnCooldownDelegate() => _signalBus.Fire(new BattleCryCooldownFinishedSignal());
}