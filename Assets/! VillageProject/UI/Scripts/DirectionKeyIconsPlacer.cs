using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DirectionKeyIconsPlacer : MonoBehaviour
{
    [Serializable]
    class IconAndDirection
    {
        public WorldToCanvasPosition Icon;
        public AnchorLinks.Direction Direction;
    }

    [SerializeField] private AnchorMode _anchorMode;
    [SerializeField] private List<IconAndDirection> _icons;
    private IDisposable _subscription;
    private Dictionary<AnchorLinks.Direction, WorldToCanvasPosition> _directionToIcon;

    private void Reset()
    {
        _anchorMode = GetComponent<AnchorMode>();
    }

    private void Awake()
    {
        _directionToIcon = _icons.ToDictionary(it => it.Direction, it => it.Icon);
    }

    private void OnEnable()
    {
        _subscription = _anchorMode.ActiveAnchor
            .Subscribe(HandleAnchorChanged)
            .AddTo(this);
        HandleAnchorChanged(_anchorMode.ActiveAnchor.CurrentValue);
    }

    private void OnDisable()
    {
        _subscription?.Dispose();
    }

    private void HandleAnchorChanged(Anchor anchor)
    {
        foreach (var icon in _directionToIcon)
        {
            icon.Value.gameObject.SetActive(false);
        }
        if (anchor == null)
        {
            return;
        }
        foreach (var link in anchor.Links.Links)
        {
            var delta = link.Anchor.transform.position - anchor.transform.position;
            var offset = delta / 2;
            if (_directionToIcon.TryGetValue(link.Direction, out var icon))
            {
                icon.gameObject.SetActive(true);
                icon.WorldPosition = anchor.transform.position + offset;
            }
        }
    }
}
