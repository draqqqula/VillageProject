using System;
using R3;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class AttackInput : IAttackInput, IInitializable, IDisposable
{
    private InputWithHolding _inputWithHolding;
    private MeleeControlsPresetManager _preset;
    
    private CompositeDisposable _disposables = new CompositeDisposable();
    private InputActionReference _action;

    public AttackInput(InputActionReference inputAction, MeleeControlsPresetManager presetManager)
    {
        _action = inputAction;
        _inputWithHolding = new InputWithHolding(inputAction);
        _preset = presetManager;
    }

    public ReadOnlyReactiveProperty<bool> IsHolding => _inputWithHolding.IsHolding;
    public ReadOnlyReactiveProperty<float> LastStarted => _inputWithHolding.LastStarted;
    public ReadOnlyReactiveProperty<float> LastEnded => _inputWithHolding.LastEnded;

    public void Initialize()
    {
        _inputWithHolding.Initialize();
        _preset.ChosenPreset.Subscribe(HandlePreset).AddTo(_disposables);
    }

    private void HandlePreset(int count)
    {
        if (count == 1)
        {
            _action.action.ApplyBindingOverride(5, "<Mouse>/rightButton");
        }
        else
        {
            _action.action.ApplyBindingOverride(5, "<None>");
        }
    }

    public void Dispose()
    {
        _inputWithHolding.Dispose();
        _disposables.Dispose();
    }
    
    public float GetUnscaledTimeSinceLastStarted()
    {
        return _inputWithHolding.GetUnscaledTimeSinceLastStarted();
    }

    public float GetUnscaledTimeSinceLastEnded()
    {
        return _inputWithHolding.GetUnscaledTimeSinceLastEnded();
    }
}