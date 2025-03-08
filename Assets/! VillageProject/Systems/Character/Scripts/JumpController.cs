using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JumpController : InputListener
{
    [SerializeField, FromInputActionAsset("Jump")] private InputActionReference _jump;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _height;
    [SerializeField] private float _duration;
    [SerializeField] private GravityAffected _gravity;

    private Vector3 JumpVector => transform.up * (_height / _duration) * Time.fixedDeltaTime;

    protected override void Reset()
    {
        base.Reset();
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        _jump.action.performed += TryJump;
    }

    private void OnDisable()
    {
        _jump.action.performed -= TryJump;
    }

    private void TryJump(InputAction.CallbackContext context)
    {
        Debug.Log(_characterController.isGrounded);
        if (_characterController.isGrounded)
        {
            StartCoroutine(MoveOverTime());
        }
    }

    private IEnumerator MoveOverTime()
    {
        _gravity.enabled = false;
        float travelled = 0;
        while (travelled < _height) 
        {
            var delta = Vector3.ClampMagnitude(JumpVector, _height - travelled);
            _characterController.Move(delta);
            travelled += delta.magnitude;
            yield return new WaitForFixedUpdate();
        }
        _gravity.enabled = true;
    }
}
