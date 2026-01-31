using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    [SerializeField] private InputActionReference _lkm;
    [SerializeField] private InputActionReference _pkm;
    private InputWithHolding _action;

    private void Awake()
    {
        _lkm.action.performed += HandlePreformed;
        _lkm.action.started += HandleStarted;
        _lkm.action.canceled += HandleCancelled;
        _action = new InputWithHolding(_lkm);
        _action.Initialize();
    }

    private void HandlePreformed(InputAction.CallbackContext callback)
    {
        Debug.Log("lkm performed");
    }

    private void HandleStarted(InputAction.CallbackContext callback)
    {
        Debug.Log("lkm started");
    }

    private void HandleCancelled(InputAction.CallbackContext callback)
    {
        Debug.Log("lkm cancelled");
    }

    private void Update()
    {
        Debug.Log(_action.IsHolding.CurrentValue);
    }
}
