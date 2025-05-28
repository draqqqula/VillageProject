using UnityEngine;

public class SpawnEffectOnDeath : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _raycastDistance;

    public void Spawn()
    {
        if (Physics.Raycast(new Ray(transform.position, Vector3.down), out var hit, _raycastDistance, _layerMask))
        {
            Instantiate(_prefab, hit.point, Quaternion.identity);
        }
    }
}
