using System;
using UnityEngine;

public interface IMarkedByIndicator
{
    public Transform OriginPoint { get; }
    public event Action OnDestroyed;
}