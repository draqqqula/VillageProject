using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AdrenalineToStaminaRate : MonoBehaviour
{
    [SerializeField] private List<float> _modifiers;
    private Adrenaline _adrenaline;
    private Stamina _stamina;
    private IDisposable _currentModifier;

    [Inject]
    public void Construct(Adrenaline adrenaline, Stamina stamina)
    {
        _adrenaline = adrenaline;
        _stamina = stamina;
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
