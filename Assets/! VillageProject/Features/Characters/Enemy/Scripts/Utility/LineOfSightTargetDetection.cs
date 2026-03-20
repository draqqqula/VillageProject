using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LineOfSightTargetDetection : MonoBehaviour
{
    [SerializeField] private SphereCollider _sphere;
    [SerializeField] private float _checkInterval;
    [SerializeField] private float _timeToForget;
    [SerializeField] private Target _target;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private TeamMember _teamMember;

    private IDictionary<Collider, Coroutine> _losChecks = new KeyValidReferenceDictionary<Collider, Coroutine>();
    private IDictionary<Collider, float> _forgetTimers = new KeyValidReferenceDictionary<Collider, float>();

    private void Reset()
    {
        _sphere = GetComponent<SphereCollider>();
        _target = GetComponentInParent<Target>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_forgetTimers.ContainsKey(other))
        {
            return;
        }
        var searcher = other.GetComponent<SearchForTarget>();
        if (searcher != null && searcher.TeamMember.Team.IsEnemiesWith(_teamMember.Team))
        {
            var coroutine = StartCoroutine(EstablishContact(other, searcher));
            if (coroutine != null)
            {
                _losChecks.Add(other, coroutine);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_losChecks.TryGetValue(other, out var coroutine))
        {
            StopCoroutine(coroutine);
            _losChecks.Remove(other);
        }
    }

    private IEnumerator EstablishContact(Collider collider, SearchForTarget searcher)
    {
        while (!HasLineOfSight(collider))
        {
            yield return new WaitForSeconds(_checkInterval);
        }
        searcher.Detect(_target);
        _losChecks.Remove(collider);
        StartCoroutine(KeepContact(collider, searcher));
    }

    private IEnumerator KeepContact(Collider collider, SearchForTarget searcher)
    {
        _forgetTimers.Add(collider, _timeToForget);
        while (collider != null)
        {
            if (HasLineOfSight(collider))
            {
                _forgetTimers[collider] = _timeToForget;
            }
            else
            {
                _forgetTimers[collider] -= _checkInterval;
                if (_forgetTimers[collider] <= 0)
                {
                    searcher.Forget(_target);
                    _forgetTimers.Remove(collider);
                    yield break;
                }
            }
            yield return new WaitForSeconds(_checkInterval);
        }
    }

    private bool HasLineOfSight(Collider collider)
    {
        var direction = collider.transform.position - transform.position;
        return Physics.Raycast(new Ray(transform.position, direction.normalized), out var hit, _sphere.radius, _layerMask) 
            && ReferenceEquals(collider, hit.collider);
    }
}