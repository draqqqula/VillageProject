using UnityEngine;
using UnityEngine.InputSystem;

public class EnterOnlyInputActiob : MonoBehaviour
{
    [SerializeField] private InputActionReference _enter;
    [SerializeField] private GameObject _target;
    
    public bool CanActivate { private get; set; }

    private void OnEnable()
    {
        _enter.action.performed += HandleEnter;
    }

    private void OnDisable()
    {
        _enter.action.performed -= HandleEnter;
    }

    private void HandleEnter(InputAction.CallbackContext context)
    {
        if (!_target.activeSelf /*&& CanActivate*/)
        {
            _enter.action.Reset();
            _target.SetActive(true);
        }
    }
}
