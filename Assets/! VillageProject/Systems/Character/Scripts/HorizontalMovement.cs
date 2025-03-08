using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HorizontalMovement : InputListener
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField, FromInputActionAsset("Move")] private InputActionReference _move;
    [SerializeField] private float _speed;

    protected override void Reset()
    {
        base.Reset();
        _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        var velocity = transform.TransformDirection(_move.action.ReadValue<Vector2>().ToVector3XZ()) * _speed;
        _characterController.Move(velocity);
    }
}