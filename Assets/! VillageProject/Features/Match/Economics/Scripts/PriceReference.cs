using R3;
using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Price", menuName = "Price")]
public class PriceReference : ScriptableObject
{
    private ReactiveProperty<bool> _avaliable = new(false);
    [field: SerializeField] public Price Value { get; private set; }
    public ReadOnlyReactiveProperty<bool> Available => _avaliable;

    public void Awake()
    {
        if (Value == null)
        {
            return;
        }
        foreach (var item in Value.Required)
        {
            item.Resource.AmountChanged += UpdateAvaliable;
        }
    }

    public void OnEnable()
    {
        if (Value == null)
        {
            return;
        }
        UpdateAvaliable();
    }

    private void UpdateAvaliable()
    {
        _avaliable.Value = Value.IsAvailable();
    }
}