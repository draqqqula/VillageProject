using R3;
using System;
using System.Collections;
using UnityEngine;

public class AndGateActivator : MonoBehaviour
{

    [SerializeField] public ObservableBehaviour SourceA;
    [SerializeField] public ObservableBehaviour SourceB;
    [SerializeField] public GameObject Target;
    private IDisposable _eventListenerA;
    private IDisposable _eventListenerB;


    private void OnEnable()
    {
        _eventListenerA = SourceA.Enabled
            .Subscribe(HandleEnabled)
            .AddTo(this);
        _eventListenerB = SourceB.Enabled
            .Subscribe(HandleEnabled)
            .AddTo(this);
    }

    private void OnDisable()
    {
        _eventListenerA?.Dispose();
        _eventListenerB?.Dispose();
    }

    private void HandleEnabled(bool value)
    {
        if (SourceA.Enabled.CurrentValue && SourceB.Enabled.CurrentValue)
        {
            Target.SetActive(true);
        }
        else
        {
            Target.SetActive(false);
        }
    }
}