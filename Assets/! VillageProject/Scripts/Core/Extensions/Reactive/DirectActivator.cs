using System;
using System.Collections;
using System.Diagnostics.Tracing;
using UnityEngine;
using R3;

public class DirectActivator : MonoBehaviour
{
    [SerializeField] public ObservableBehaviour Source;
    [SerializeField] public GameObject Target;
    private IDisposable _eventListener;

    private void OnEnable()
    {
        _eventListener = Source.Enabled
            .Subscribe(HandleEnabled)
            .AddTo(this);
    }

    private void OnDisable()
    {
        _eventListener?.Dispose();
    }

    private void HandleEnabled(bool value)
    {
        if (value)
        {
            Target.SetActive(true);
        }
        else
        {
            Target.SetActive(false);
        }
    }
}