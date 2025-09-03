using System.Collections;
using UnityEngine;

public class ProjectileGravity : MonoBehaviour
{
    private const float TargetAngle = 90f;
    [SerializeField] private float _speed;

    private float SpeedScaled => _speed * Time.fixedDeltaTime;

    void FixedUpdate()
    {
        if (transform.localRotation.eulerAngles.x == TargetAngle)
        {
            enabled = false;
            return;
        }
        var cached = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(Mathf.MoveTowardsAngle(transform.eulerAngles.x, TargetAngle, SpeedScaled), 
            transform.localRotation.eulerAngles.y, 
            transform.localRotation.eulerAngles.z);

        var delta = (transform.localRotation.eulerAngles - cached).magnitude;
        Debug.Log(delta);
    }
}