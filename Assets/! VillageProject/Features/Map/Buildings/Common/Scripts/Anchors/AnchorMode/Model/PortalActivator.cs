using UnityEngine;

public class PortalActivator : MonoBehaviour
{
    [SerializeField] private AnchorMode _mode;

    public void Portal()
    {
        if (_mode.ActiveAnchor.CurrentValue.gameObject.TryGetComponent<Portal>(out var portal))
        {
            portal.Teleport();
        }
    }
}