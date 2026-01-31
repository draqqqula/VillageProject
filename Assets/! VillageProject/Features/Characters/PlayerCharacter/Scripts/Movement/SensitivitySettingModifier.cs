using R3;
using System;
using System.Collections;
using UnityEngine;


public class SensitivitySettingModifier : MonoBehaviour
{
    [SerializeField] private FirstPersonController _firstPerson;
    [SerializeField] private MovementConfiguration _configuration;
    private IDisposable _multiplier;

    private void Awake()
    {
        _configuration.Updated.Subscribe(HandleUpdated).AddTo(this);
    }

    private void HandleUpdated(int _)
    {
        _multiplier?.Dispose();
        _multiplier = _firstPerson.Sensitivity.AddMultiplier(_configuration.Sensitivity);
    }
}