using UnityEngine;

public class AdrenalineGainDisplay : SignalListener<StaminaSignalInvoker.AdrenalineChangedSignal>
{
    [SerializeField] private AdrenalineGainEffect _vfx;
    [SerializeField] private AnimationCurve _amountToSize;
    private float _cachedValue;

    protected override void OnSignal(StaminaSignalInvoker.AdrenalineChangedSignal signal)
    {
        if (signal.Value > _cachedValue)
        {
            var effect = Instantiate(_vfx, transform);
            var size = _amountToSize.Evaluate(signal.Value - _cachedValue);
            effect.SetSize(size);
        }
        _cachedValue = signal.Value;
    }
}
