using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class Healthbar : MonoBehaviour
{
    private const string VariableName = "value";

    [SerializeField] private LocalizeStringEvent _localizeStringEvent;
    [SerializeField] private Health _health;
    private IntVariable _variable;

    private void Reset()
    {
        _localizeStringEvent = GetComponentInChildren<LocalizeStringEvent>();
    }

    private void Start()
    {
        if (_localizeStringEvent.StringReference.TryGetValue(VariableName, out var variable))
        {
            _variable = (IntVariable)variable;
        }
        UpdateValue();
    }

    private void OnEnable()
    {
        _health.OnDamageDealt += HandleHealthUpdated;
    }

    private void OnDisable()
    {
        _health.OnDamageDealt -= HandleHealthUpdated;
    }

    public void HandleHealthUpdated(float amount)
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        _variable.Value = Convert.ToInt32(_health.Amount);
    }
}
