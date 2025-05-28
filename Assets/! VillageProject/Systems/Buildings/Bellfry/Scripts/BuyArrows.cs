using R3;
using System;
using UnityEngine;
using static BuyArrows;

public class BuyArrows : BuildingMenuItemBase<BuyArrowsData>
{
    [SerializeField] private BuyArrowsData _data;
    public override ReadOnlyReactiveProperty<BuyArrowsData> Data => new ReactiveProperty<BuyArrowsData>(_data);

    public override ReadOnlyReactiveProperty<bool> Available => new ReactiveProperty<bool>(true);

    [Serializable]
    public class BuyArrowsData
    {
        [SerializeField] public Price Price;
        [SerializeField] public ResourceAmount Amount;
    }

    public override void ShowPreview(GameObject ui)
    {
    }

    public override void HidePreview(GameObject ui)
    {
    }

    public override bool TryPerform()
    {
        if (_data.Price.TryPay())
        {
            _data.Amount.Resource.Increment(_data.Amount.Amount);
            return true;
        }
        return false;
    }
}
