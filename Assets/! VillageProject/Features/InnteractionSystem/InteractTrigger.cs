using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(Collider))]
public class InteractTrigger : MonoBehaviour
{
    [Inject] InteractTip _interactTip;
    [SerializeField] private string _interactKey;
    [SerializeField] private InputActionReference _actionReference;
    
    [SerializeField] private GameObject _interactableObject;

    private bool _isOnTrigger;

    private void Start()
    {
        _actionReference.action.performed += OnActionPerformed;
    }
    
    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        if (_isOnTrigger && _interactableObject.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_isOnTrigger) return;
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _isOnTrigger = true;
            _interactTip.ShowTip("взаимодействовать", _interactKey);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isOnTrigger) return;
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _isOnTrigger = false;
            _interactTip.HideTip();
        }
    }
    
    private void OnDestroy()
    {
        _actionReference.action.performed -= OnActionPerformed;
    }
}