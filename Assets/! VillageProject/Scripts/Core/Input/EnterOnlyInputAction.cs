using UnityEngine;
using UnityEngine.InputSystem;

public class EnterOnlyInputActiob : MonoBehaviour
{
    [SerializeField] private InputActionReference _enter;
    [SerializeField] protected GameObject _target;
    
    private void OnEnable()
    {
        _enter.action.performed += HandleEnter;
    }

    private void OnDisable()
    {
        _enter.action.performed -= HandleEnter;
    }

    protected virtual void HandleEnter(InputAction.CallbackContext context)
    {
        if (!_target.activeSelf)
        {
            _enter.action.Reset();
            _target.SetActive(true);
        }
    }
}