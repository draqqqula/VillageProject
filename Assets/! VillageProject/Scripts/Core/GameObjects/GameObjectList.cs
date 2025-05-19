using R3.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R3;

public class GameObjectList : MonoBehaviour
{

    protected List<GameObject> _items = new List<GameObject>();
    public IReadOnlyList<GameObject> Items => _items;
    public virtual GameObject InstantiateElement(GameObject prefab)
    {
        var item = Instantiate(prefab, transform);
        return Add(item);
    }

    public virtual GameObject Add(GameObject item)
    {
        item.transform.parent = transform;
        item.OnDestroyAsObservable().Subscribe(value => HandleDestroyed(item));
        _items.Add(item);
        return item;
    }

    public virtual IEnumerable<GameObject> InstantiateElementRange(IEnumerable<GameObject> prefabs)
    {
        var instances = InstantiateRange(prefabs);
        _items.AddRange(instances);
        return instances;
    }

    private IEnumerable<GameObject> InstantiateRange(IEnumerable<GameObject> prefabs)
    {
        foreach (var prefab in prefabs)
        {
            var item = Instantiate(prefab, transform);
            item.OnDestroyAsObservable().Subscribe(value => HandleDestroyed(item));
            yield return item;
        }
    }

    private void HandleDestroyed(GameObject item)
    {
        _items.Remove(item);
    }
}