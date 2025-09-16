using R3;
using System;
using System.Collections;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    class RateModifier : IDisposable
    {
        public Stamina Stamina;
        public float Value;

        public void Dispose()
        {
            if (Stamina == null)
            {
                return;
            }
            Stamina._rateModifier /= Value;
        }
    }

    class ZeroRateModifier : IDisposable
    {
        public Stamina Stamina;

        public void Dispose()
        {
            if (Stamina == null)
            {
                return;
            }
            Stamina._zeroModifiers -= 1;
        }
    }

    [SerializeField] private float _rate;
    [SerializeField] private float _border;
    [field: SerializeField] public float MaxValue { get; private set; }
    [field: SerializeField] public float CooldownDuration { get; private set; }

    private float _rateModifier = 1;
    private int _zeroModifiers = 0;
    private ReactiveProperty<float> _value = new ReactiveProperty<float>();
    public ReadOnlyReactiveProperty<float> Value => _value;
    public ReadOnlyReactiveProperty<bool> IsOnCooldown { get; private set; }


    public bool TrySpend(float required)
    {
        if (_value.Value > 0)
        {
            var desired = _value.Value - required;
            if (desired < _border)
            {
                StartCoroutine(WaitForCooldown());
            }
            _value.Value = Math.Max(desired, 0);
            return true;
        }
        return false;
    }

    public IDisposable ModifyRate(float modifier)
    {
        if (modifier == 0)
        {
            _zeroModifiers++;
            return new ZeroRateModifier()
            {
                Stamina = this
            };
        }
        _rateModifier *= modifier;
        return new RateModifier()
        {
            Value = modifier,
            Stamina = this
        };
    }

    private IEnumerator WaitForCooldown()
    {
        var modifier = ModifyRate(0);
        yield return new WaitForSeconds(CooldownDuration);
        modifier.Dispose();
    }


    private void Awake()
    {
        IsOnCooldown = _value.Select(it => it == 0).ToReadOnlyReactiveProperty();
    }

    private void FixedUpdate()
    {
        if (_value.Value >= MaxValue || _zeroModifiers > 0)
        {
            return;
        }
        var delta = _rate * Time.fixedDeltaTime * _rateModifier;
        if (delta == 0)
        {
            return;
        }
        _value.Value = Math.Min(_value.Value + delta, MaxValue);
    }
}
