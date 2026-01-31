using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BlockInput : IBlockInput, IInitializable, IDisposable
{
    private InputWithHolding _inputWithHolding;
    private MeleeControlsPresetManager _preset;
    
    private CompositeDisposable _disposables = new CompositeDisposable();
    private InputActionReference _action;

    public BlockInput(InputActionReference inputAction, MeleeControlsPresetManager presetManager)
    {
        _action = inputAction;
        _inputWithHolding = new InputWithHolding(inputAction);
        _preset = presetManager;
    }

    public ReadOnlyReactiveProperty<bool> IsHolding => _inputWithHolding.IsHolding;
    public ReadOnlyReactiveProperty<float> LastStarted => _inputWithHolding.LastStarted;
    public ReadOnlyReactiveProperty<float> LastEnded => _inputWithHolding.LastEnded;
    public ReactiveProperty<float> CurrentHoldTime => _inputWithHolding.CurrentHoldTime;

    public void Initialize()
    {
        _inputWithHolding.Initialize();
        _preset.ChosenPreset.Subscribe(HandlePreset).AddTo(_disposables);
    }
    
    private void HandlePreset(int count)
    {
        if (count == 1)
        {
            _action.action.ApplyBindingOverride(1, "<Keyboard>/leftShift");
        }
        else
        {
            Debug.Log(_action.action.bindings[1]);
            _action.action.ApplyBindingOverride(1, "<Mouse>/rightButton");
        }
    }
    
    public void Dispose()
    {
        _inputWithHolding.Dispose();
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