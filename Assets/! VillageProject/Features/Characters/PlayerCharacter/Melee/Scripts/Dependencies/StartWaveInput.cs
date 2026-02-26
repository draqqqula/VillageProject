using System;
using R3;
using UnityEngine.InputSystem;
using Zenject;

public class StartWaveInput : IStartWaveInput, IInitializable, IDisposable
{
    private InputWithHolding _inputWithHolding;
    
    public ReadOnlyReactiveProperty<bool> IsHolding => _inputWithHolding.IsHolding;

    public StartWaveInput(InputActionReference inputAction)
    {
        _inputWithHolding = new InputWithHolding(inputAction);
    }
    
    public void Initialize()
    {
        _inputWithHolding.Initialize();
    }
    
    public void Dispose()
    {
        _inputWithHolding.Dispose();
    }
}