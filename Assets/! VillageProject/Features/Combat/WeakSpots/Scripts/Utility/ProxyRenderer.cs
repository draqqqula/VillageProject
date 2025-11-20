using UnityEngine;

public class ProxyRenderer : MonoBehaviour
{
    [SerializeField] private Renderer _targetRenderer;


    private void OnBecameVisible()
    {
        _targetRenderer.enabled = true;
    }

    private void OnBecameInvisible() 
    {
        _targetRenderer.enabled = false;
    }
}
