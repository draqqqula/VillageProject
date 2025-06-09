using UnityEngine;
using UnityEngine.InputSystem;

public class ExitOnlyInputAction : MonoBehaviour
{
    [SerializeField] private InputActionReference _exit;
    [SerializeField] private GameObject _target;

    private void OnEnable()
    {
        _exit.action.performed += HandleExit;
    }

    private void OnDisable()
    {
        _exit.action.performed -= HandleExit;
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
