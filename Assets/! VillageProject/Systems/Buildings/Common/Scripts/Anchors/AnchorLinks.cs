
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
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
                link.Anchor.Links.ValidateVisual();
                continue;
            }
            otherLinks.Remove(mirrored);
            otherLinks.Remove(opposite);
            otherLinks.Add(new LinkedAnchor(oppositeDirection, anchor));
            link.Anchor.Links.ValidateVisual();
        }
        ValidateVisual();
#if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }

    private void ValidateVisual()
    {
        var display = gameObject.GetComponentInChildren<LinkDisplay>(true);
        display.gameObject.layer = LayerMask.NameToLayer("Overlay");
        display.GenerateLinks();
        var vfx = transform.Find("States/Visible/VFX").gameObject;
        foreach (var child in vfx.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Overlay");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(child.gameObject);
#endif
        }
        vfx.layer = LayerMask.NameToLayer("Overlay");
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(vfx);
        UnityEditor.EditorUtility.SetDirty(display);
        UnityEditor.EditorUtility.SetDirty(display.gameObject);
#endif
    }
}
