using R3;
using UnityEngine;

public class AmmunitionStorage : MonoBehaviour
{
    [SerializeField] private TowerDamageOverTime _tower;
    private ReactiveProperty<TowerAmmunition> _ammunition = new ReactiveProperty<TowerAmmunition>();
    private ReactiveProperty<uint> _amount = new ReactiveProperty<uint>(0);
    public ReadOnlyReactiveProperty<ResourceAmount> Amount { get; private set; }

    private void Awake()
    {
        _tower.ProjectileFired += HandleProjectileFired;
        Amount = _amount
            .CombineLatest(_ammunition, (amount, ammunition) => new ResourceAmount(ammunition?.Resource, amount))
            .ToReadOnlyReactiveProperty();
    }

    private void OnDestroy()
    {
        _tower.ProjectileFired -= HandleProjectileFired;
    }

    public bool IsSameAmmunition(TowerAmmunition ammunition)
    {
        return Object.ReferenceEquals(ammunition, _ammunition.CurrentValue);
    }

    public bool CanStore(TowerAmmunition ammunition, uint amount)
    {
        return _ammunition == null || !(IsSameAmmunition(ammunition) && _amount.CurrentValue + amount > _ammunition.Value.MaxAmount);
    }

    public bool TryStore(TowerAmmunition ammunition, uint amount)
    {
        if (!CanStore(ammunition, amount))
        {
            return false;
        }
        if (IsSameAmmunition(ammunition))
        {
            _amount.Value += amount;
        }
        else
        {
            DisposeAmmunition();
            _amount.Value = amount;
            _ammunition.Value = ammunition;
        }    
        _tower.Spawner = ammunition.Spawner;
        return true;
    }

    private void DisposeAmmunition()
    {
        if (_ammunition.Value == null)
        {
            return;
        }
        _ammunition.Value.Resource.Increment(_amount.Value);
        _amount.Value = 0;
        _ammunition.Value = null;
    }

    private void HandleProjectileFired()
    {
        _amount.Value -= 1;
        if (_amount.Value == 0)
        {
            _ammunition.Value = null;
            _tower.Spawner = null;
        }
    }
}
