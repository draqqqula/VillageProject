using R3;
using System.Collections.Generic;
using UnityEngine;

public class SubscriberBehaviour : MonoBehaviour
{

    [SerializeField] private ObservableBehaviour _source;

    private void Awake()
    {
        _source.Enabled
            .Subscribe(HandleEnabled)
            .AddTo(this);
    }

    private void HandleEnabled(bool value)
    {
        gameObject.SetActive(value);
    }
}