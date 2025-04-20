using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerDamageOverTime : MonoBehaviour
{
    private const string TargetTag = "damageable";

    [SerializeField] private float _interval;
    [SerializeField] private DamageSource _damage;
    private Coroutine _shooting;
    private Dictionary<Collider, Health> _targets = new Dictionary<Collider, Health>();

    private void OnEnable()
    {
        _shooting = StartCoroutine(ShootOverTime());
    }

    private void OnDisable()
    {
        StopCoroutine(_shooting);
        _shooting = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TargetTag))
        {
            _targets.Add(other, other.GetComponent<Health>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TargetTag))
        {
            _targets.Remove(other);
        }
    }

    private IEnumerator ShootOverTime()
    {
        while (true)
        {
            foreach (var target in _targets.Values)
            {
                target.Deal(_damage);
                Debug.Log($"Tower dealt damage to {target.gameObject.name}");
            }
            yield return new WaitForSeconds(_interval);
        }
    }
}
