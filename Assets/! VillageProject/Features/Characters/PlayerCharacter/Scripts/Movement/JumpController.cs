using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JumpController : InputListener
{
    [SerializeField, FromInputActionAsset("Jump")] private InputActionReference _jump;
    [SerializeField] private CharacterVelocity _velocity;
    [SerializeField] private GroundDetectorBase _groundDetector;
    [SerializeField] private float _height;
    [SerializeField] private float _duration;
    [SerializeField] private float _factor = 1;
    [SerializeField] private GravityAffected _gravity;
    private Vector3 JumpDirection => transform.up;

    private void Reset()
    {
        _velocity = GetComponent<CharacterVelocity>();
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
        if (_groundDetector.IsGrounded)
        {
            StartCoroutine(MoveOverTime());
        }
    }

    private IEnumerator MoveOverTime()
    {
        _gravity.enabled = false;
        float target = 0;
        float traveled = 0;
        float elapsed = 0;

        while (elapsed < _duration) 
        {
            target = _height * Mathf.Pow(elapsed / _duration, _factor);

            var delta = target - traveled;

            var velocity = JumpDirection * delta;
            _velocity.Add(velocity);

            traveled = target;

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _gravity.enabled = true;
    }
}
