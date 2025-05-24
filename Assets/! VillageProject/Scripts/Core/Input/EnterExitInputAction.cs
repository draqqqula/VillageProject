using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnterExitInputAction : MonoBehaviour
{
    [SerializeField] private InputActionReference _enter;
    [SerializeField] private InputActionReference _exit;
    [SerializeField] private GameObject _target;

    private void OnEnable()
    {
        _enter.action.performed += HandleEnter;
        _exit.action.performed += HandleExit;
    }

    private void OnDisable()
    {
        _enter.action.performed -= HandleEnter;
        _exit.action.performed -= HandleExit;
    }

    private void HandleEnter(InputAction.CallbackContext context)
    {
        if (!_target.activeSelf)
        {
            _enter.action.Reset();
            _target.SetActive(true);
        }
    }

    private void HandleExit(InputAction.CallbackContext context) 
    {
        if (_target.activeSelf)
        {
            _exit.action.Reset();
            _target.SetActive(false);
        }
    }
}