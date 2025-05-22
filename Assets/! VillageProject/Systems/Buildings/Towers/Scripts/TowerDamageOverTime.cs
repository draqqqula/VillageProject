using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerDamageOverTime : MonoBehaviour
{
    private const string TargetTag = "damageable";

    public event Action ProjectileFired;
    [SerializeField] private TeamMember _team;
    [SerializeField] private float _defaultInterval;
    private Coroutine _shooting;
    private IDictionary<Collider, Health> _targets = new KeyValidReferenceDictionary<Collider, Health>();
    public ProjectileSpawner Spawner { get; set; }

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
                yield return new WaitForSeconds(ShootClosestTarget());
            }
            else
            {
                _shooting = null;
                yield break;
            }
        }
    }

    private float ShootClosestTarget()
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

        if (closest != null && Spawner != null)
        {
            var projectile = Spawner.Spawn(closest.transform, transform);
            ProjectileFired?.Invoke();
            return projectile;
        }
        return _defaultInterval;
    }
}
