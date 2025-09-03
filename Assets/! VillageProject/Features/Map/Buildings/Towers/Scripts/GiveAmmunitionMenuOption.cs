using R3;
using UnityEngine;
using UnityEngine.Localization;
using static GiveAmmunitionMenuOption;

public class GiveAmmunitionMenuOption : BuildingMenuItemBase<GiveAmmunitionData>
{
    private ReadOnlyReactiveProperty<GiveAmmunitionData> _data;
    [SerializeField] private LocalizedString _label;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private uint _amount;
    [field: SerializeField] public AmmunitionStorage Storage { get; private set; }
    [field: SerializeField] public PriceReference Price {  get; private set; }
    [field: SerializeField] public TowerAmmunition Ammunition { get; private set; }
    public override ReadOnlyReactiveProperty<GiveAmmunitionData> Data => Storage.Amount
            .CombineLatest(Ammunition.Resource.AmountReactive,
            (storageAmount, resourceAmount) =>
            new GiveAmmunitionData(storageAmount.Resource, Ammunition.Resource, resourceAmount, storageAmount.Amount, _amount, Ammunition.MaxAmount, _label, _sprite))
            .ToReadOnlyReactiveProperty()
            .AddTo(this);
    public override ReadOnlyReactiveProperty<bool> Available => Price.Available
        .Concat(Storage.Amount.Select(it => Storage.CanStore(Ammunition, _amount)))
        .ToReadOnlyReactiveProperty()
        .AddTo(this);

    public class GiveAmmunitionData
    {
        public ResourceVariable StorageResource;
        public ResourceVariable BuyResource;
        public uint ResourceAmount;
        public uint StorageAmount;
        public uint BuyAmount;
        public uint MaxAmount;
        public LocalizedString Label;
        public Sprite Icon;

        public GiveAmmunitionData(
            ResourceVariable storageResource, 
            ResourceVariable buyResource, 
            uint resourceAmount, 
            uint storageAmount, 
            uint buyAmount,
            uint maxAmount,
            LocalizedString label, 
            Sprite sprite)
        {
            StorageResource = storageResource;
            BuyResource = buyResource;
            ResourceAmount = resourceAmount;
            StorageAmount = storageAmount;
            BuyAmount = buyAmount;
            MaxAmount = maxAmount;
            Label = label;
            Icon = sprite;
        }
    }

    public override void HidePreview(GameObject ui)
    {
    }

    public override void ShowPreview(GameObject ui)
    {
    }

    public override bool TryPerform()
    {
        if (Storage.CanStore(Ammunition, _amount))
        {
            if (Price.Value.TryPay())
            {
                Storage.TryStore(Ammunition, _amount);
                return true;
            }
        }
        return false;
    }
}
