using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StraightLineProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask _surfaces;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _penetrationDepth;
    public UnityEvent OnSurfaceHit;

    private float ScaledSpeed => _speed * Time.fixedDeltaTime;

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, ScaledSpeed, _surfaces))
        {
            transform.position = hit.point + transform.forward * _penetrationDepth;
            OnSurfaceHit?.Invoke();
            enabled = false;
            return;
        }
        transform.position += transform.forward * _speed * Time.fixedDeltaTime;
    }
}