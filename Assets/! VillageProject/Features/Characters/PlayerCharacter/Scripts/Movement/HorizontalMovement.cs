using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

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
    
    private const float MOVE_PARAM_SPEED_MULTIPLIER = 5;
    
    [Inject] private Animator _animator;
    [Inject] private MoveParamUpdater _moveParamUpdater;
    
    public ModifiableValue<float> SpeedModifier { get; private set; } = new ModifiableValue<float>(1f);

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
        _velocity.Add(_direction * _speed * SpeedModifier.Value.CurrentValue);

        var moveParamSpeed = MOVE_PARAM_SPEED_MULTIPLIER * _velocity.Velocity.CurrentValue.ToXZ().magnitude;
        
        if (input.magnitude > 0) _moveParamUpdater.IncreaseParam(Time.deltaTime * moveParamSpeed);
        else _moveParamUpdater.ReleaseParam(Time.deltaTime * (MOVE_PARAM_SPEED_MULTIPLIER * _maxSpeed - moveParamSpeed));
        
        _animator.SetFloat("Move", _moveParamUpdater.MoveParameter.CurrentValue);
        
        if (_moveParamUpdater.MoveParameter.CurrentValue == 0) _animator.SetBool("Walking", false);
        else _animator.SetBool("Walking", true);
        
        if (_moveParamUpdater.MoveParameter.CurrentValue == 1) _moveParamUpdater.ReleaseParam();
    }
}