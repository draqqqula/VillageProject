using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class VariableDisplay : MonoBehaviour
{
    private const string VariableName = "value";

    [SerializeField] private LocalizeStringEvent _localizeStringEvent;
    [SerializeField] private ResourceVariable _resource;
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
        _resource.AmountChanged += HandleHealthUpdated;
    }

    private void OnDisable()
    {
        _resource.AmountChanged -= HandleHealthUpdated;
    }

    public void HandleHealthUpdated()
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        _variable.Value = Convert.ToInt32(_resource.Amount);
    }
}