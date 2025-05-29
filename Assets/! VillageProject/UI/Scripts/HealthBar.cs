using R3;
using System;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private GameObject _heartPrefab;

    private void Start()
    {
        _health.AmountReactive
            .Subscribe(UpdateDisplay)
            .AddTo(this);
    }

    private void UpdateDisplay(float amount)
    {
        if (amount < 0)
        {
            return;
        }
        var targetCount = Convert.ToInt32(amount);
        var delta = transform.childCount - targetCount;
        if (delta < 0)
        {
            for (var i = 0; i < -delta; i++)
            {
                Instantiate(_heartPrefab, transform);
            }
        }
        else if (delta > 0)
        {
            for (var i = 0; i < delta; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
