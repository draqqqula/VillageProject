using System.Collections.Generic;
using UnityEngine;

public class InputAggregator : MonoBehaviour
{
    [SerializeField] private List<InputListener> _listeners;
    private void Reset()
    {
        GetComponentsInChildren(_listeners);
    }

    private void OnEnable()
    {
        foreach (var listener in _listeners)
        {
            listener.enabled = true;
        }
    }

    private void OnDisable()
    {
        foreach (var listener in _listeners)
        {
            listener.enabled = false;
        }
    }
}
