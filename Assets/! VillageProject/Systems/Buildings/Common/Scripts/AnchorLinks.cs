
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnchorLinks : MonoBehaviour
{
    public enum Direction
    {
        Left, Right, Up, Down, None
    }

    [Serializable]
    public class LinkedAnchor
    {
        [field: SerializeField] public Direction Direction { get; private set; }
        [field: SerializeField] public Anchor Anchor { get; private set; }
    }

    [field: SerializeField] public List<LinkedAnchor> Links { get; set; } = new();
    private Dictionary<Direction, Anchor> _directionToAnchor;

    private void Awake()
    {
        _directionToAnchor = Links.ToDictionary(it => it.Direction, it => it.Anchor);
    }

    public bool TryGetLinkTo(Direction direction, out Anchor anchor)
    {
        return _directionToAnchor.TryGetValue(direction, out anchor);
    }
}
