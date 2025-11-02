using System;
using UnityEngine;

public class HitboxEvent : MonoBehaviour
{
    public event Action OnHit;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<DamagingHitbox>(out var hitbox))
        {
            OnHit?.Invoke();
        }
    }
}