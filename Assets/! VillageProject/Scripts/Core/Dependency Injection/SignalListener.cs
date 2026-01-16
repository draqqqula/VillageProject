using System;
using System.Collections;
using UnityEngine;
using Zenject;

public abstract class SignalListener<T> : MonoBehaviour
{
    protected SignalBus SignalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        SignalBus = signalBus;
        signalBus.Subscribe<T>(OnSignal);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<T>(OnSignal);
    }
    
    protected abstract void OnSignal(T signal);
}

public abstract class SignalListener<T1, T2> : MonoBehaviour
{
    protected SignalBus SignalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        SignalBus = signalBus;
        signalBus.Subscribe<T1>(OnSignal);
        signalBus.Subscribe<T2>(OnSignal);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<T1>(OnSignal);
        SignalBus.Unsubscribe<T2>(OnSignal);
    }
    
    protected abstract void OnSignal(T1 signal);
    protected abstract void OnSignal(T2 signal);
}

public abstract class SignalListener<T1, T2, T3> : MonoBehaviour
{
    protected SignalBus SignalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        SignalBus = signalBus;
        signalBus.Subscribe<T1>(OnSignal);
        signalBus.Subscribe<T2>(OnSignal);
        signalBus.Subscribe<T3>(OnSignal);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<T1>(OnSignal);
        SignalBus.Unsubscribe<T2>(OnSignal);
        SignalBus.Unsubscribe<T3>(OnSignal);
    }
    
    protected abstract void OnSignal(T1 signal);
    protected abstract void OnSignal(T2 signal);
    protected abstract void OnSignal(T3 signal);
}