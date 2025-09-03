using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorNavigation : InputListener
{
    [SerializeField] private AnchorMode _view;
    [SerializeField, FromInputActionAsset("Navigate")] public InputActionReference Navigate;

    private void Reset()
    {
        _view = GetComponentInParent<AnchorMode>();
    }

    private void OnEnable()
    {
        Navigate.action.performed += HandleNaviagation;
    }

    private void OnDisable()
    {
        Navigate.action.performed -= HandleNaviagation;
    }

    private void HandleNaviagation(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        if (_view.ActiveAnchor.CurrentValue.Links.TryGetLinkTo(AnchorExtensions.GetDirection(value), out var linked))
        {
            _view.MoveTo(linked);
        }
    }
}