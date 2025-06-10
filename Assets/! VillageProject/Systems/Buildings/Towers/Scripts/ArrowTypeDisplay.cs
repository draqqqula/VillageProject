using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTypeDisplay : MonoBehaviour
{
    [Serializable]
    public class AmmuitionToIcon
    {
        public ResourceVariable Resource;
        public MapIcon Icon;
    }

    [SerializeField] private AmmunitionStorage _storage;
    [SerializeField] private List<AmmuitionToIcon> _icons;
    private IDisposable _subscription;

    private void Start()
    {
        _subscription = _storage.Amount.Subscribe(HandleResourceChanged).AddTo(this);
    }


    private void HandleResourceChanged(ResourceAmount amount)
    {
        foreach (var icon in _icons)
        {
            if (amount != null && amount.Resource == icon.Resource && amount.Amount > 0)
            {
                icon.Icon.enabled = true;
            }
            else
            {
                icon.Icon.enabled = false;
            }
        }
    }
}
