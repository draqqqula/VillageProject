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

    public ResourceVariable Resource
    {
        get
        {
            return _resource;
        }
        set
        {
            if (_resource != null)
            {
                _resource.AmountChanged -= HandleHealthUpdated;
            }
            _resource = value;
            if (_resource != null)
            {
                _resource.AmountChanged += HandleHealthUpdated;
            }
        }
    }

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
        if (_resource == null)
        {
            return;
        }
        _resource.AmountChanged += HandleHealthUpdated;
    }

    private void OnDisable()
    {
        if (_resource == null)
        {
            return;
        }
        _resource.AmountChanged -= HandleHealthUpdated;
    }

    public void HandleHealthUpdated()
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        if (_resource == null)
        {
            return;
        }
        _variable.Value = Convert.ToInt32(_resource.Amount);
    }
}