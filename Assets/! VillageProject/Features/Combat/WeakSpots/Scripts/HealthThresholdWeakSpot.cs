using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HealthThresholdWeakSpot : MonoBehaviour
{
    [SerializeField] protected float _threshold;
    [SerializeField] protected float _delay = 0.3f;
    [SerializeField] protected Health _health;
    [SerializeField] protected WeakSpotController _bodyRoot;

    private void OnEnable()
    {
        _health.OnDamageDealt += HandleDamageDealt;
    }

    private void OnDisable()
    {
        _health.OnDamageDealt -= HandleDamageDealt;
    }

    public virtual void HandleDamageDealt(float amount)
    {
        if (_health.Amount <= 0) return;
        
        if (!_bodyRoot.IsOpened.CurrentValue && _health.Amount <= _threshold)
        {
            StartCoroutine(SpawnWeakSpotWithDelay());
        }
    }

    private IEnumerator SpawnWeakSpotWithDelay()
    {
        yield return new WaitForSeconds(_delay);
        _bodyRoot.Open();
    }
}