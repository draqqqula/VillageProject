using R3;
using System;
using System.Collections;
using System.Collections.Generic;

public class CompositeDisposableBase : ICollection<IDisposable>, IDisposable
{
    private CompositeDisposable _innerDisposable = new CompositeDisposable();

    public int Count => _innerDisposable.Count;

    public bool IsReadOnly => _innerDisposable.IsReadOnly;

    public void Add(IDisposable item)
    {
        _innerDisposable.Add(item);
    }

    public void Clear()
    {
        _innerDisposable.Clear();
    }

    public bool Contains(IDisposable item)
    {
        return _innerDisposable.Contains(item);
    }

    public void CopyTo(IDisposable[] array, int arrayIndex)
    {
        _innerDisposable.CopyTo(array, arrayIndex);
    }

    public virtual void Dispose()
    {
        _innerDisposable.Dispose();
    }

    public IEnumerator<IDisposable> GetEnumerator()
    {
        return _innerDisposable.GetEnumerator();
    }

    public bool Remove(IDisposable item)
    {
        return _innerDisposable.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _innerDisposable.GetEnumerator();
    }
}