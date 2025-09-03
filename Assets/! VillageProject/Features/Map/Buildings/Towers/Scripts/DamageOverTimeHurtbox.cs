using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeHurtbox : MonoBehaviour
{
    [SerializeField] private DamageSource _damage;
    [SerializeField] private float _interval;
    private HashSet<Collider> _colliders = new HashSet<Collider>();
    private HashSet<Health> _activeCoroutines = new HashSet<Health>();

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<Health>();
        if (health != null)
        {
            _colliders.Add(other);
            if (!_activeCoroutines.Contains(health))
            {
                StartCoroutine(DealOverTime(other, health));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
    }

    private IEnumerator DealOverTime(Collider collider, Health health)
    {
        _activeCoroutines.Add(health);
        while (_colliders.Contains(collider))
        {
            health.Deal(_damage);
            yield return new WaitForSeconds(_interval);
        }
        _activeCoroutines.Remove(health);
    }
}
