using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    [SerializeField] private GameObject _sfx;
    [SerializeField] private List<Collider> _bodyParts;
    private Collider _collider;
    private GameObject _effect;

    private void Reset()
    {
        GetComponentsInChildren(_bodyParts);
    }

    public bool IsOpened => _collider != null;

    public void Open()
    {
        if (IsOpened)
        {
            return;
        }
        var index = Random.Range(0, _bodyParts.Count);
        _collider = _bodyParts[index];
        _effect = Instantiate(_sfx, _collider.bounds.center, Quaternion.identity, _collider.transform);
    }

    public void Close()
    {
        if (!IsOpened)
        {
            return;
        }
        _collider = null;
        Destroy(_effect);
    }

    public bool Raycast(Ray ray, float maxDistance)
    {
        if (!IsOpened)
        {
            return false;
        }
        return _collider.Raycast(ray, out var hitInfo, maxDistance);
    }
}
