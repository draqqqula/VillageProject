using R3;
using System;
using UnityEngine;

public class TargetActivator : MonoBehaviour
{
    [SerializeField] public GameObject Target;
    private IDisposable _eventListener;

    private void OnEnable()
    {
        var anchor = GetComponentInParent<Anchor>();
        _eventListener = anchor.Active
            .Subscribe(HandleActivation)
            .AddTo(this);
    }

    private void OnDisable()
    {
        _eventListener?.Dispose();
    }

    private void HandleActivation(bool value)
    {
        Target.SetActive(value);
    }
}