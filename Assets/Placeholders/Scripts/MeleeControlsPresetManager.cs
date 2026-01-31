using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeControlsPresetManager
{
    private int _count;
    private InputActionReference _action;
    private ReactiveProperty<int> _chosenPreset;

    public MeleeControlsPresetManager(int count, InputActionReference action)
    {
        _count = count;
        _action = action;
        _action.action.performed += HandlePress;
        _chosenPreset = new ReactiveProperty<int>(0);
    }

    public ReadOnlyReactiveProperty<int> ChosenPreset => _chosenPreset;

    private void HandlePress(InputAction.CallbackContext context)
    {
        if (_chosenPreset.Value + 1 == _count)
        {
            _chosenPreset.Value = 0;
        }
        else
        {
            _chosenPreset.Value += 1;
        }
    }
}
