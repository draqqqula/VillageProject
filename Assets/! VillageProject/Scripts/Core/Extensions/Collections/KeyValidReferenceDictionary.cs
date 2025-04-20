using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class KeyValidReferenceDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : UnityEngine.Object
{
    private readonly IDictionary<TKey, TValue> _inner = new Dictionary<TKey, TValue>();

    private IEnumerable<KeyValuePair<TKey, TValue>> GetUpdatedItems()
    {
        foreach (var item in _inner.ToArray())
        {
            if (item.Key == null)
            {
                _inner.Remove(item.Key);
                continue;
            }
            yield return item;
        }
    }

    public TValue this[TKey key] { get => _inner[key]; set => _inner[key] = value; }

    public ICollection<TKey> Keys => GetUpdatedItems().Select(it => it.Key).AsReadOnlyCollection();

    public ICollection<TValue> Values => GetUpdatedItems()
        .Select(it => it.Value)
        .AsReadOnlyCollection();

    public int Count => GetUpdatedItems().Count();

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        _inner.Add(key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _inner.Add(item.Key, item.Value);
    }

    public void Clear()
    {
        _inner.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _inner.Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
        return _inner.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _inner.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return GetUpdatedItems().GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        return _inner.Remove(key);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return _inner.Remove(item);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _inner.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
