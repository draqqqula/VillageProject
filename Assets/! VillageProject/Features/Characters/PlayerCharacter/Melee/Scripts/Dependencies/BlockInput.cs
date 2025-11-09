using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BlockInput : IBlockInput, IInitializable, IDisposable
{
    private InputWithHolding _inputWithHolding;

    public BlockInput(InputActionReference inputAction)
    {
        _inputWithHolding = new InputWithHolding(inputAction);
    }

    public ReadOnlyReactiveProperty<bool> IsHolding => _inputWithHolding.IsHolding;
    public ReadOnlyReactiveProperty<float> LastStarted => _inputWithHolding.LastStarted;
    public ReadOnlyReactiveProperty<float> LastEnded => _inputWithHolding.LastEnded;
    public ReactiveProperty<float> CurrentHoldTime => _inputWithHolding.CurrentHoldTime;

    public void Initialize()
    {
        _inputWithHolding.Initialize();
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