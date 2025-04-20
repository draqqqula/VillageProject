using System.Collections;
using UnityEngine;

public class DamagingProjectile : MonoBehaviour
{
    [SerializeField] private DamageSource _damage;
    [SerializeField] private TravellingProjectile _traveling;
    
    public void Deal()
    {
        var health = _traveling.Destination.GetComponent<Health>();
        if (health != null)
        {
            health.Deal(_damage);
        }
    }
}