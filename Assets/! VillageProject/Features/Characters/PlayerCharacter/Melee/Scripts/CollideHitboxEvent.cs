using System;
using UnityEngine;

public class CollideHitboxEvent : MonoBehaviour
{
    public event Action<GameObject> OnHit;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger) OnHit?.Invoke(other.gameObject);
    }
}