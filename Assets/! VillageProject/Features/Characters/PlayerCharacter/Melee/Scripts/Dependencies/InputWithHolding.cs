using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputWithHolding : IDisposable
{
    private ReactiveProperty<bool> _isHolding = new ReactiveProperty<bool>(false);
    private ReactiveProperty<float> _lastStarted = new ReactiveProperty<float>(0f);
    private ReactiveProperty<float> _lastEnded = new ReactiveProperty<float>(0f);
    public InputActionReference _inputAction;
    
    private ReactiveProperty<float> _currentHoldTime = new ReactiveProperty<float>(0f);
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    public InputWithHolding(InputActionReference inputAction)
    {
        _inputAction = inputAction;
    }

    public ReadOnlyReactiveProperty<bool> IsHolding => _isHolding;
    public ReadOnlyReactiveProperty<float> LastStarted => _lastStarted;
    public ReadOnlyReactiveProperty<float> LastEnded => _lastEnded;
    public ReactiveProperty<float> CurrentHoldTime => _currentHoldTime;
    
    public void Initialize()
    {
        _inputAction.action.started += _ => HandleStarted();
        _inputAction.action.canceled += _ => HandleCancelled();
        _isHolding.Value = _inputAction.action.phase == InputActionPhase.Started;
        
        Observable.EveryUpdate()
            .Where(_ => _isHolding.Value)
            .Subscribe(_ => UpdateHoldTime())
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    private void HandleStarted()
    {
        _lastStarted.Value = Time.unscaledTime;
        _currentHoldTime.Value = 0f;
        _isHolding.Value = true;
    }
    
    private void HandleCancelled()
    {
        _lastEnded.Value = Time.unscaledTime;
        _isHolding.Value = false;
    }
    
    private void UpdateHoldTime()
    {
        _currentHoldTime.Value = GetUnscaledTimeSinceLastStarted();
    }

    public float GetUnscaledTimeSinceLastStarted()
    {
        return Time.unscaledTime - _lastStarted.Value;
    }

    public float GetUnscaledTimeSinceLastEnded()
    {
        return Time.unscaledTime - _lastEnded.Value;
    }
}