using R3.Triggers;
using System.Collections.Generic;
using UnityEngine;
using R3;

public class ExpandableLayout : GameObjectList
{
    [SerializeField] private List<LayoutHelper> _layoutUpdaters;

    public void UpdateLayout()
    {
        foreach (var layout in _layoutUpdaters)
        {
            layout.Rebuild();
        }
    }
}
