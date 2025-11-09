using R3;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ShiftInput : IShiftInput, IInitializable, IDisposable
{
    [Inject] private SwingConfiguration _swingConfiguration;
    private InputWithHolding _inputWithHolding;

    public ReadOnlyReactiveProperty<bool> IsHolding => _inputWithHolding.IsHolding;

    public void Dispose()
    {
        _inputWithHolding.Dispose();
    }

    public void Initialize()
    {
        _inputWithHolding = new InputWithHolding(_swingConfiguration.ShiftInput);
        _inputWithHolding.Initialize();
    }
}