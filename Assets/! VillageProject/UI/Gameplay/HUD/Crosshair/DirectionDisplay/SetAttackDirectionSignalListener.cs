using System.Collections;
using UnityEngine;
using Zenject;

public class SetAttackDirectionSignalListener : IInitializable
{
    private DirectionDisplay _directionDisplay;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus, DirectionDisplay directionDisplay)
    {
        _directionDisplay = directionDisplay;
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<SetAttackDirectionSignal>(OnSignal);
        _signalBus.Subscribe<ShowAttackDirectionSignal>(OnSignal);
        _signalBus.Subscribe<AttackDirectionIntensitySignal>(OnSignal);
    }

    protected void OnSignal(SetAttackDirectionSignal signal)
    {
        if (signal.Direction == AttackDirection.None)
        {
            return;
        }
        _directionDisplay.Direction = signal.Direction;
    }

    protected void OnSignal(ShowAttackDirectionSignal signal)
    {
        _directionDisplay.gameObject.SetActive(signal.Visible);
    }

    protected void OnSignal(AttackDirectionIntensitySignal signal)
    {
        _directionDisplay.SetIntensity(signal.Value);
    }
}