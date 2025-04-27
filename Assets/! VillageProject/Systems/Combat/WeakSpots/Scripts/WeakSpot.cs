using System.Collections.Generic;
using UnityEngine;

public class WeakSpot : MonoBehaviour
{
    [SerializeField] private GameObject _sfx;
    [SerializeField] private List<Collider> _bodyParts;
    private Collider _collider;

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
        var spot = _bodyParts[index];
        var effect = Instantiate(_sfx, spot.bounds.center, Quaternion.identity, spot.transform);
        _collider = effect.GetComponent<Collider>();
    }

    public void Close()
    {
        if (!IsOpened)
        {
            return;
        }
        Destroy(_collider.gameObject);
        _collider = null;
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
