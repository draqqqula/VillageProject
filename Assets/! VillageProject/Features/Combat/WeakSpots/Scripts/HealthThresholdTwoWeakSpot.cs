using System.Collections;
using UnityEngine;

public class HealthThresholdTwoWeakSpot : HealthThresholdWeakSpot
{
    [SerializeField] private float _threshold2;
    [SerializeField] private WeakSpotController _weakSpotController2;

    private bool _isFirstPointOpened;
    
    public override void HandleDamageDealt(float amount)
    {
        if (_health.Amount <= 0) return;
        
        if (!_isFirstPointOpened && !_bodyRoot.IsOpened.CurrentValue && _health.Amount <= _threshold)
        {
            StartCoroutine(SpawnWeakSpotWithDelay(_bodyRoot));
            _isFirstPointOpened = true;
        }

        if (!_weakSpotController2.IsOpened.CurrentValue && _health.Amount <= _threshold2)
        {
            StartCoroutine(SpawnWeakSpotWithDelay(_weakSpotController2));
        }
    }
    
    private IEnumerator SpawnWeakSpotWithDelay(WeakSpotController weakSpotController)
    {
        yield return new WaitForSeconds(_delay);
        weakSpotController.Open();
    }
}