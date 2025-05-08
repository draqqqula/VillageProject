using System.Collections.Generic;
using UnityEngine;

public class TargetAnchor : MonoBehaviour
{
    [SerializeField] private AnchorMode _mode;
    [SerializeField] private AnchorView _targetView;
    [SerializeField] private AnchorView _commonView;
    [SerializeField] private List<InputListener> inputListeners = new List<InputListener>();

    private void Reset()
    {
        _mode = GetComponent<AnchorMode>();
    }

    private void OnEnable()
    {
        _targetView.enabled = true;

    }

    private void OnDisable()
    {
        _targetView.enabled = false;
    }
}