using System.Collections;
using System.Linq;
using UnityEngine;

public class SmokeSignal : TowerIntervalAction
{
    [SerializeField] private float _interval;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private SphereCollider _sphere;
    protected override float Perform()
    {
        if (Range.Targets.Keys.Any(IsUnobscured))
        {
            if (!_particles.isPlaying)
            {
                _particles.Play();
            }
        }
        else
        {
            if (_particles.isPlaying)
            {
                _particles.Stop();
            }
        }
        return _interval;
    }

    protected override void HandleBreak()
    {
        Perform();
    }

    private bool IsUnobscured(Collider collider)
    {
        var direction = (collider.transform.position - transform.position).normalized;
        return collider.Raycast(new Ray(transform.position, direction), out var hit, _sphere.radius);
    }
}
