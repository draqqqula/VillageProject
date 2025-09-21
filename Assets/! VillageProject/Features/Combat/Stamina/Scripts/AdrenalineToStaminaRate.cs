using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AdrenalineToStaminaRate
{
    private List<float> _modifiers;
    [Inject] private Adrenaline _adrenaline;
    [Inject] private Stamina _stamina;
    private IDisposable _currentModifier;

    public AdrenalineToStaminaRate(List<float> modifiers)
    {
        _modifiers = modifiers;
        Stage = _adrenaline.Value.Select(AdrenalineToStage).ToReadOnlyReactiveProperty();
        Modifier = Stage.Select(StageToModifier).ToReadOnlyReactiveProperty();
        Modifier.Subscribe(HandleModifierChanged);
    }

    public int StageCount => _modifiers.Count;
    public ReadOnlyReactiveProperty<int> Stage { get; private set; }
    public ReadOnlyReactiveProperty<float> Modifier { get; private set; }
    public float StageDuration => _adrenaline.MaxValue / StageCount;
    public IEnumerable<float> Modifiers => _modifiers;

    private int AdrenalineToStage(float adrenaline)
    {
        return Math.Min(Convert.ToInt32(Mathf.Floor(adrenaline / _adrenaline.MaxValue * StageCount)), StageCount - 1);
    }

    private float StageToModifier(int stage)
    {
        return _modifiers[stage];
    }

    private void HandleModifierChanged(float modifier)
    {
        if (_currentModifier != null)
        {
            _currentModifier.Dispose();
            _currentModifier = null;
        }
        if (modifier != 1)
        {
            _currentModifier = _stamina.ModifyRate(modifier);
        }
    }
}
