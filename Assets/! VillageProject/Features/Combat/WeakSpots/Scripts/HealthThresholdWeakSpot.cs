using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HealthThresholdWeakSpot : MonoBehaviour
{
    [SerializeField] private float _threshold;
    [SerializeField] private Health _health;
    [SerializeField] private WeakSpotController _bodyRoot;

    private void OnEnable()
    {
        _health.OnDamageDealt += HandleDamageDealt;
    }

    private void OnDisable()
    {
        _health.OnDamageDealt -= HandleDamageDealt;
    }

    public void HandleDamageDealt(float amount)
    {
        if (!_bodyRoot.IsOpened && _health.Amount <= _threshold)
        {
            _bodyRoot.Open();
            enabled = false;
        }
    }
}