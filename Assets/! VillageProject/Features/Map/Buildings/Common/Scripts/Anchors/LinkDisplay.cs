using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using UnityEngine;

public class LinkDisplay : MonoBehaviour
{
    [SerializeField] private Anchor _anchor;
    [SerializeField] public GameObject Prefab;
    [SerializeField] private List<GameObject> _generated;

    private void Reset()
    {
        _anchor = GetComponentInParent<Anchor>();
    }

    [ContextMenu("generate")]
    public void GenerateLinks()
    {
        foreach (var link in _generated)
        {
            DestroyImmediate(link.gameObject);
        }
        foreach (var child in transform.EnumerateImmediateChildren().ToArray())
        {
            DestroyImmediate(child);
        }
        _generated = new List<GameObject>();

        foreach (var link in _anchor.Links.Links)
        {
            var name = link.Anchor.gameObject.name + "To" +
                _anchor.gameObject.name + "Link_generated";
            var obj = Instantiate(Prefab, transform);
            obj.name = name;
            var line = obj.GetComponent<LineRenderer>();
            obj.layer = gameObject.layer;
            line.positionCount = 2;
            line.SetPosition(0, _anchor.transform.position);
            line.SetPosition(1, link.Anchor.transform.position);
            _generated.Add(obj);
        }
    }
}
