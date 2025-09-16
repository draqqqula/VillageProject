using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalineToStaminaRate : MonoBehaviour
{
    public class AdrenalineStage
    {
        public float Adrenaline;
        public float Modifier;
    }

    [SerializeField] private Adrenaline _adrenaline;
    [SerializeField] private Stamina _stamina;
    [SerializeField] private int _stageCount;
    private IDisposable _currentModifier;

    public ReadOnlyReactiveProperty<int> Stage { get; private set; }
    public ReadOnlyReactiveProperty<float> Modifier { get; private set; }
    public float StageDuration => _adrenaline.MaxValue / _stageCount;

    public void Start()
    {
        Stage = _adrenaline.Value.Select(AdrenalineToStage).ToReadOnlyReactiveProperty();
        Modifier = Stage.Select(StageToModifier).ToReadOnlyReactiveProperty();
        Modifier.Subscribe(HandleModifierChanged);
    }

    private int AdrenalineToStage(float adrenaline)
    {
        return Math.Min(Convert.ToInt32(Mathf.Floor(adrenaline / _adrenaline.MaxValue * _stageCount)), _stageCount - 1);
    }

    private float StageToModifier(int stage)
    {
        return stage + 1;
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
