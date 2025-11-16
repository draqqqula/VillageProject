using System.Collections;
using UnityEngine;
using Zenject;

public class FloatingCrosshairSignalListener : IInitializable
{
    private FloatingCrosshair _floatingCrosshair;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus, FloatingCrosshair floatingCrosshair)
    {
        _floatingCrosshair = floatingCrosshair;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<FloatingCrosshairStateSignal>(OnSignal);
        _signalBus.Subscribe<FloatingCrosshairPositionSignal>(OnSignal);
    }

    protected void OnSignal(FloatingCrosshairStateSignal signal)
    {
        _floatingCrosshair.gameObject.SetActive(signal.Enabled);
    }

    protected void OnSignal(FloatingCrosshairPositionSignal signal)
    {
        _floatingCrosshair.EulerAngles = signal.EulerAngles;
    }
}