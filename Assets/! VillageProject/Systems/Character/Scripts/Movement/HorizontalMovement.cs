using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HorizontalMovement : InputListener
{
    [SerializeField] private CharacterVelocity _velocity;
    [SerializeField, FromInputActionAsset("Move")] private InputActionReference _move;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] protected float _deceleration;
    [SerializeField] private float _movingDelta;
    private float _speed = 0;
    private Vector3 _direction = Vector3.zero;

    private void Reset()
    {
        _velocity = GetComponent<CharacterVelocity>();
    }

    private void FixedUpdate()
    {
        var input = transform.TransformDirection(_move.action.ReadValue<Vector2>().ToVector3XZ());

        var targetSpeed = _maxSpeed * input.magnitude;

        if (_speed < targetSpeed)
        {
            _speed = Mathf.Min(_speed + _acceleration, targetSpeed);
        }
        else
        {
            _speed = Mathf.Max(_speed - _deceleration, targetSpeed);
        }

        _direction = Vector3.MoveTowards(_direction, input, _movingDelta);
        _velocity.Add(_direction * _speed);
    }
}