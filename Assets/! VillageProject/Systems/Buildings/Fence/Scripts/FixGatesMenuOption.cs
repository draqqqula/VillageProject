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
    [SerializeField] private DamageSource _healing;
    [SerializeField] private GateState _gateState;

    private float MaxHealth
    {
        get
        {
            if (_health.ComponentProvider.TryGetComponent<MaxHealthComponent>(out var maxHealth))
            {
                return maxHealth.MaxHealth;
            }
            return 0;
        }
    }

    public override ReadOnlyReactiveProperty<FixGatesData> Data => _health.AmountReactive.Select(it =>
    {
        return new FixGatesData()
        {
            PriceToFix = _price.Value,
            IsFullHealth = it == MaxHealth
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
        if (_health.Amount < MaxHealth && _price.Value.TryPay())
        {
            _health.enabled = true;
            _health.gameObject.SetActive(true);
            _health.Deal(_healing);
            _gateState.Fix();
            return true;
        }
        return false;
    }
}