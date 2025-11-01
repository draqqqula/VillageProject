using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DamagingHitbox : MonoBehaviour
{
    private const string DamageableTag = "damageable";

    [SerializeField] private DamageInteractable _source;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(DamageableTag))
        {
            return;
        }

        var target = other.GetComponent<IDamageInteractable>();
        DamageInteraction.Interact(target, _source);
    }
}
