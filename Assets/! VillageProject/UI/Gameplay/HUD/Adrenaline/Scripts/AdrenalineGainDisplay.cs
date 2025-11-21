using UnityEngine;

public class AdrenalineGainDisplay : SignalListener<StaminaSignalInvoker.AdrenalineChangedSignal>
{
    [SerializeField] private AdrenalineGainEffect _vfx;
    private float _cachedValue;

    protected override void OnSignal(StaminaSignalInvoker.AdrenalineChangedSignal signal)
    {
        if (signal.Value > _cachedValue)
        {
            var effect = Instantiate(_vfx, transform);
            effect.SetSize(signal.Value - _cachedValue);
        }
        _cachedValue = signal.Value;
    }
}
