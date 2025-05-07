using R3;
using System;
using UnityEngine;

public class ActivateOnSelected : MonoBehaviour
{
    [SerializeField] public GameObject Target;
    private void Awake()
    {
        var anchor = GetComponentInParent<Anchor>();
        anchor.Active
            .Subscribe(HandleActivation)
            .AddTo(this);
    }

    private void HandleActivation(bool value)
    {
        Target.SetActive(value);
    }
}