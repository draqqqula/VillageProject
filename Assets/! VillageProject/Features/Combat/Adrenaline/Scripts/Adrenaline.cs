using R3;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Adrenaline : MonoBehaviour
{
    [SerializeField] private float _defaultRate;
    [SerializeField] private AnimationCurve _decreaseSpeedOverValue;
    private ReactiveProperty<float> _value = new ReactiveProperty<float>(0);
    private ReactiveProperty<bool> _isOnCooldown = new ReactiveProperty<bool>(false);

    [field: SerializeField] public float MaxValue { get; private set; }
    public ReadOnlyReactiveProperty<float> Value => _value;
    public ReadOnlyReactiveProperty<bool> IsOnCooldown => _isOnCooldown;

    private void FixedUpdate()
    {
        if (_isOnCooldown.Value)
        {
            return;
        }
        var rate = _defaultRate * _decreaseSpeedOverValue.Evaluate(Value.CurrentValue / MaxValue) * Time.fixedDeltaTime;
        _value.Value = Math.Max(_value.Value - rate, 0);
    }

    public void Gain(float amount, float cooldown)
    {
        _value.Value = Math.Min(_value.Value + amount, MaxValue);
        StartCoroutine(WaitForCooldown(cooldown));
    }

    private IEnumerator WaitForCooldown(float duration)
    {
        _isOnCooldown.Value = true;
        yield return new WaitForSeconds(duration);
        _isOnCooldown.Value = false;
    }
}
