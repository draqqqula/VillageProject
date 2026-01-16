using R3;
using System.Collections;
using UnityEngine;

public class FixGatesMenuOption : BuildingMenuItemBase<FixGatesMenuOption.FixGatesData>
{
    public class FixGatesData
    {
        public bool IsFullHealth;
        public Price PriceToFix;
    }

    [SerializeField] private Health _health;
    [SerializeField] private PriceReference _price;
    [SerializeField] private GateState _gateState;

    public override ReadOnlyReactiveProperty<FixGatesData> Data => _health.AmountReactive.Select(it =>
    {
        return new FixGatesData()
        {
            PriceToFix = _price.Value,
            IsFullHealth = it == _health.MaxHealth
        };
    })
        .ToReadOnlyReactiveProperty();

    public override ReadOnlyReactiveProperty<bool> Available => new ReactiveProperty<bool>();

    public override void HidePreview(GameObject ui)
    {
    }

    public override void ShowPreview(GameObject ui)
    {
    }

    public override bool TryPerform()
    {
        if (_health.Amount < _health.MaxHealth && _price.Value.TryPay())
        {
            _health.enabled = true;
            _health.gameObject.SetActive(true);
            _health.Amount = _health.MaxHealth;
            _gateState.Fix();
            return true;
        }
        return false;
    }
}