using System;
using UnityEngine;

public interface IWorkEventSource<T>
{
    public event Action<T> OnFinished;
}
