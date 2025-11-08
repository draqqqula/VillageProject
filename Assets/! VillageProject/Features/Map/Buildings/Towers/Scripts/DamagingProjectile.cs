using System.Collections;
using UnityEngine;

public class DamagingProjectile : MonoBehaviour
{
    [SerializeField] private DamageInteractable _source;
    [SerializeField] private TravellingProjectile _traveling;
    
    public void Deal()
    {
        var target = _traveling.Destination.GetComponent<IDamageInteractable>();
        if (target != null)
        {
            DamageInteraction.Interact(target, _source);
        }
    }
}