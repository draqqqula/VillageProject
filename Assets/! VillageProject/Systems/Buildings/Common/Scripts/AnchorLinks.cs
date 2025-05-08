
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
        public LinkedAnchor(Direction direction, Anchor anchor)
        {
            Direction = direction;
            Anchor = anchor;
        }
        [field: SerializeField] public Direction Direction { get; private set; }
        [field: SerializeField] public Anchor Anchor { get; private set; }
    }

    [field: SerializeField] public List<LinkedAnchor> Links { get; set; }
    private Dictionary<Direction, Anchor> _directionToAnchor;

    private void Awake()
    {
        _directionToAnchor = Links.ToDictionary(it => it.Direction, it => it.Anchor);
    }

    public bool TryGetLinkTo(Direction direction, out Anchor anchor)
    {
        return _directionToAnchor.TryGetValue(direction, out anchor);
    }

    [ContextMenu("Mirror links")]
    public void Mirror()
    {
        var anchor = GetComponent<Anchor>();
        foreach (var link in Links)
        {
            var otherLinks = link.Anchor.Links.Links;
            var oppositeDirection = link.Direction.Opposite();
            var mirrored = otherLinks.FirstOrDefault(it => it.Anchor == anchor);
            var opposite = otherLinks.FirstOrDefault(it => it.Direction == oppositeDirection);
            if (mirrored == opposite && mirrored != null)
            {
                continue;
            }
            otherLinks.Remove(mirrored);
            otherLinks.Remove(opposite);
            otherLinks.Add(new LinkedAnchor(oppositeDirection, anchor));
        }
    }
}
