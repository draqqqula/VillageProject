using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    public event Action<Target> ForgetByAll;
    [field: SerializeField] public int Priority {  get; private set; }

    private void OnDisable()
    {
        ForgetByAll?.Invoke(this);
    }
}