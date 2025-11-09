using System.Collections.Generic;
using UnityEngine;

public class DisableAllShadowCasters : MonoBehaviour
{
    [SerializeField] private List<Renderer> _renderers;

    private void Reset()
    {
        GetComponentsInChildren(_renderers);
    }

    [ContextMenu("Disable all")]
    public void DisableAll()
    {
        SetAll(false);
    }

    [ContextMenu("Enable all")]
    public void EnableAll()
    {
        SetAll(true);
    }

    private void SetAll(bool value)
    {
        foreach (Renderer renderer in _renderers)
        {
            renderer.shadowCastingMode = value ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
