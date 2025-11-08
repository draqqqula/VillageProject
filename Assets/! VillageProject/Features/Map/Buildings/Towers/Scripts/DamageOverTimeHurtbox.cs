using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeHurtbox : MonoBehaviour
{
    [SerializeField] private DamageInteractable _source;
    [SerializeField] private float _interval;
    private HashSet<Collider> _colliders = new HashSet<Collider>();
    private HashSet<IDamageInteractable> _activeCoroutines = new HashSet<IDamageInteractable>();

    private void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<IDamageInteractable>();
        if (target != null)
        {
            _colliders.Add(other);
            if (!_activeCoroutines.Contains(target))
            {
                StartCoroutine(DealOverTime(other, target));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _colliders.Remove(other);
    }

    private IEnumerator DealOverTime(Collider collider, IDamageInteractable target)
    {
        _activeCoroutines.Add(target);
        while (_colliders.Contains(collider))
        {
            DamageInteraction.Interact(target, _source);
            yield return new WaitForSeconds(_interval);
        }
        _activeCoroutines.Remove(target);
    }
}
