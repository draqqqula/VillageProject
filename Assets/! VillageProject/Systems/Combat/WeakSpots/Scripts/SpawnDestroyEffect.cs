using UnityEngine;

public class SpawnDestroyEffect : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    private void OnDestroy()
    {
        Instantiate(_prefab, transform.position, transform.rotation, transform.parent);
    }
}
