using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorView : InputListener
{
    [SerializeField] private Camera _anchorCamera;
    [SerializeField] private Anchor _defaultAnchor;
    [SerializeField, FromInputActionAsset("Navigate")] public InputActionReference Navigate;
    [SerializeField] private float _cameraElevation;
    public Anchor ActiveAnchor { get; private set; }

    private void OnEnable()
    {
        _anchorCamera.gameObject.SetActive(true);
        Navigate.action.performed += HandleNaviagation;
        Time.timeScale = 0.0f;

        MoveTo(_defaultAnchor);
    }

    private void OnDisable()
    {
        _anchorCamera.gameObject.SetActive(false);
        Navigate.action.performed -= HandleNaviagation;
        Time.timeScale = 1.0f;

        if (ActiveAnchor != null)
        {
            ActiveAnchor.Active.Value = false;
        }
        ActiveAnchor = null;
    }

    private void MoveTo(Anchor anchor)
    {
        if (ActiveAnchor != null)
        {
            ActiveAnchor.Active.Value = false;
        }
        ActiveAnchor = anchor;
        ActiveAnchor.Active.Value = true;
        var position = anchor.transform.position;
        _anchorCamera.transform.position = new Vector3(position.x, _cameraElevation, position.z);
    }

    private void HandleNaviagation(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        if (ActiveAnchor.Links.TryGetLinkTo(GetDirection(value), out var linked))
        {
            MoveTo(linked);
        }
    }

    private AnchorLinks.Direction GetDirection(Vector2 vector)
    {
        if (vector == Vector2.up)
        {
            return AnchorLinks.Direction.Up;
        }
        else if (vector == Vector2.down)
        {
            return AnchorLinks.Direction.Down;
        }
        else if (vector == Vector2.right)
        {
            return AnchorLinks.Direction.Right;
        }
        else if (vector == Vector2.left)
        {
            return AnchorLinks.Direction.Left;
        }
        else
        {
            return AnchorLinks.Direction.None;
        }
    }
}
