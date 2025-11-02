using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class DamagingHitbox : MonoBehaviour
{
    private const string DamageableTag = "damageable";

    [SerializeField] private DamageSource _damageSource;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(DamageableTag))
        {
            return;
        }

        var health = other.GetComponent<Health>();
        if (health != null && health.isActiveAndEnabled)
        {
            health.Deal(_damageSource);
        }
    }
}
