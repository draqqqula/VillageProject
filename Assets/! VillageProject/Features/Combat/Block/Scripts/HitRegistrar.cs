using System;
using UnityEngine;

public abstract class HitRegistrar : IDisposable
{
    public abstract event Action OnHit;

    public abstract void Activate();
    public abstract void Deactivate();
    
    public abstract void Dispose();
}