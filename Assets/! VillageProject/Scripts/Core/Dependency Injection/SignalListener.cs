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