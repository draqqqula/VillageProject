using System;
using UnityEngine;

public class ManualVelocityMovementAgent 
{
    private AnimationCurve _curve;
    private float _maxTime;
    private float _speed;
    
    private float _currentTime;
    
    private CharacterController _characterController;
    private Vector3 _direction;
    public Vector3 CurrentVelocity => _characterController.velocity;

    private bool _isMoveWithCurve;
    private bool _isMoving;
    public event Action OnFinished;

    public ManualVelocityMovementAgent(AnimationCurve curve, CharacterController characterController)
    {
        _curve = curve;
        _characterController = characterController;
    }
    
    public void StartMovement(Vector3 target, float speed, float movementTime, bool isMoveWithCurve = true)
    {
        if (_isMoving) return;
        
        _maxTime = movementTime;
        _isMoveWithCurve = isMoveWithCurve;
        
        _direction = (target - _characterController.transform.position).normalized;
        _direction.y = -1f;
        _speed = speed;
        
        _isMoving = true;
        _currentTime = 0;
    }

    public void StopMovement()
    {
        if (!_isMoving) return;
        
        _direction = Vector3.zero;
        _isMoving = false;
        _currentTime = 0;
        OnFinished?.Invoke();
    }
    
    public void Update()
    {
        if (_isMoving)
        {
            if (_isMoveWithCurve) MoveWithCurve(_direction);
            else Move(_direction);
            
            _currentTime += Time.deltaTime;
            if (_currentTime >= _maxTime) StopMovement();
        }
    }
    
    private void MoveWithCurve(Vector3 direction)
    {
        var velocity = direction * _speed * _curve.Evaluate(_currentTime / _maxTime);
        _characterController.Move(velocity * Time.deltaTime);
    }

    private void Move(Vector3 direction)
    {
        var velocity = direction * _speed;
        _characterController.Move(velocity * Time.deltaTime);
    }
}