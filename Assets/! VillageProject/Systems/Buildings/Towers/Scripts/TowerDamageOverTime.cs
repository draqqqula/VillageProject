using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerDamageOverTime : MonoBehaviour
{
    private const string TargetTag = "damageable";

    [SerializeField] private float _interval;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private TeamMember _team;
    private Coroutine _shooting;
    private IDictionary<Collider, Health> _targets = new KeyValidReferenceDictionary<Collider, Health>();

    private void OnDisable()
    {
        if (_shooting != null)
        {
            StopCoroutine(_shooting);
        }
        _shooting = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TargetTag))
        {
            var health = other.GetComponent<Health>();
            var targetTeamMember = other.GetComponent<TeamMember>();
            if (health != null 
                && targetTeamMember != null
                && _team.Team.IsEnemiesWith(targetTeamMember.Team))
            {
                _targets.Add(other, health);

                if (_shooting == null)
                {
                    _shooting = StartCoroutine(ShootOverTime());
                }
            }
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
            if (_targets.Count != 0)
            {
                ShootClosestTarget();
            }
            else
            {
                _shooting = null;
                yield break;
            }

            yield return new WaitForSeconds(_interval);
        }
    }

    private void ShootClosestTarget()
    {
        var min = float.MaxValue;
        Health closest = null;
        foreach (var target in _targets.Values)
        {
            var distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance < min)
            {
                min = distance;
                closest = target;
            }
        }

        if (closest != null)
        {
            var arrow = Instantiate(_projectile, transform);
            var projectile = arrow.GetComponent<TravellingProjectile>();
            projectile.SetPath(transform, closest.transform);

            Debug.Log($"Tower dealt damage to {closest.gameObject.name}");
        }
    }
}
