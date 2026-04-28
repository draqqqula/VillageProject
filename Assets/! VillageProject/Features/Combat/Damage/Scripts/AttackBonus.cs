using R3;

public class AttackBonus
{
    public ReadOnlyReactiveProperty<float> DamageMultiplierAmount => _damageMultiplierAmount;
    private ReactiveProperty<float> _damageMultiplierAmount = new ReactiveProperty<float>(1);

    public float DamageMultiplier
    {
        get => _damageMultiplierAmount.Value;
        set => _damageMultiplierAmount.Value = value;
    }

    public float DefaultDamageMultiplier => 1f;
}