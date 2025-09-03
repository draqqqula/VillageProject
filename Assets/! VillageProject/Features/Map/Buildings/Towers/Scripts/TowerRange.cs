using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    private const string TargetTag = "damageable";

    public event Action<Health> OnTargetEnter;
    public event Action<Health> OnTargetExit;
    [SerializeField] private TeamMember _team;
    private KeyValidReferenceDictionary<Collider, Health> _targets = new KeyValidReferenceDictionary<Collider, Health>();
    public IReadOnlyDictionary<Collider, Health> Targets => _targets;

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

                OnTargetEnter?.Invoke(health);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TargetTag))
        {
            if (_targets.TryGetValue(other, out var health))
            {
                if (_targets.Remove(other))
                {
                    OnTargetExit?.Invoke(health);
                }
            }
        }
    }
}