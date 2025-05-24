using System.Collections;
using UnityEngine;
using Zenject;

class PrefabToComponentFactory<T> : IFactory<Transform, T> where T : MonoBehaviour
{
    private GameObject _prefab;
    public PrefabToComponentFactory(GameObject prefab)
    {
        _prefab = prefab;
    }
    public T Create(Transform transform)
    {
        var go = Object.Instantiate(_prefab, transform);
        return go.GetComponent<T>();
    }
}