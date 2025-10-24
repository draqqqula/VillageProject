using R3;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class AttackInput : IAttackInput, IInitializable
{
    private ReactiveProperty<bool> _isHolding = new ReactiveProperty<bool>(false);
    private ReactiveProperty<float> _lastStarted = new ReactiveProperty<float>(0f);
    private ReactiveProperty<float> _lastEnded = new ReactiveProperty<float>(0f);
    public InputActionReference _inputAction;

    public AttackInput(InputActionReference inputAction)
    {
        _inputAction = inputAction;
    }

    public ReadOnlyReactiveProperty<bool> IsHolding => _isHolding;
    public ReadOnlyReactiveProperty<float> LastStarted => _lastStarted;
    public ReadOnlyReactiveProperty<float> LastEnded => _lastEnded;

    public void Initialize()
    {
        _inputAction.action.started += _ => HandleStarted();
        _inputAction.action.canceled += _ => HandleCancelled();
        _isHolding.Value = _inputAction.action.phase == InputActionPhase.Started;
    }

    private void HandleStarted()
    {
        _lastStarted.Value = Time.unscaledTime;
        _isHolding.Value = true;
    }

    private void HandleCancelled()
    {
        _lastEnded.Value = Time.unscaledTime;
        _isHolding.Value = false;
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