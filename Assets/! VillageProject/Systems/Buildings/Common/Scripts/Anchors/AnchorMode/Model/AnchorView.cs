using UnityEngine;
using R3;
using System;

public class AnchorView : MonoBehaviour
{
    [SerializeField] private AnchorMode _mode;
    private IDisposable _eventListener;

    private void Reset()
    {
        _mode = GetComponentInParent<AnchorMode>();
    }

    private void OnEnable()
    {
        MoveTo(_mode.ActiveAnchor.CurrentValue);
        _eventListener = _mode.ActiveAnchor.Subscribe(MoveTo);
    }

    private void OnDisable()
    {
        _eventListener?.Dispose();
    }

    private void MoveTo(Anchor active)
    {
        transform.position = active.transform.position;
    }
}
