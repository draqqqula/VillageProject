using UnityEngine;

public class SpawnDestroyEffect : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    private void OnDestroy()
    {
        if (transform.parent != null)
        {
            Instantiate(_prefab, transform.position, transform.rotation, transform.parent);
        }
    }
}
