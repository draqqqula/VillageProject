using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class BuildingInfo : ObservableBehaviour
{
    [SerializeField] private List<BuildingMenuItemBase> _options;
    [field: SerializeField] public LocalizedString Name { get; private set; }
    public IEnumerable<BuildingMenuItemBase> Options => _options;

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        GetComponentsInChildren(_options);
    }
}