using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollRoot : MonoBehaviour
{
    private const string Layer = "Bodies";

    [SerializeField] private List<Rigidbody> _rigidbodies;

    private void Reset()
    {
        GetComponentsInChildren(_rigidbodies);
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.gameObject.layer = LayerMask.NameToLayer(Layer);
        }
        OnDisable();
    }

    private void OnEnable()
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
    }

    private void OnDisable()
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    private void OnDestroy()
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
    }

    [ContextMenu("Fill")]
    private void Fill()
    {
        GetComponentsInChildren(_rigidbodies);
    }
}