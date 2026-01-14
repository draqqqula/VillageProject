using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HealthThresholdWeakSpot : MonoBehaviour
{
    [SerializeField] private float _threshold;
    [SerializeField] private float _delay = 0.3f;
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
        if (!_bodyRoot.IsOpened.CurrentValue && _health.Amount <= _threshold)
        {
            StartCoroutine(SpawnWeakSpotWithDelay());
        }
    }

    private IEnumerator SpawnWeakSpotWithDelay()
    {
        yield return new WaitForSeconds(_delay);
        _bodyRoot.Open();
        enabled = false;
        StopAllCoroutines();
    }
}