using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GravityAffected : MonoBehaviour
{
    [SerializeField] private CharacterVelocity _velocity;
    [SerializeField] private GroundDetectorBase _groundDetector;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxValue;
    [SerializeField] private float _minValue;
    [SerializeField] private float _gravity = 0;

    private float AccelerationPerFrame => _acceleration * Time.fixedDeltaTime;

    private void Reset()
    {
        _velocity = GetComponent<CharacterVelocity>();
    }

    private void FixedUpdate()
    {
        if (_groundDetector.IsGrounded)
        {
            _gravity = _minValue;
        }
        else
        {
            _gravity = Mathf.Min(_gravity + AccelerationPerFrame, _maxValue);
        }
        _velocity.Add(Vector3.down * _gravity);
    }
}
