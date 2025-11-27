using UnityEngine;

public class AdrenalineGainDisplay : SignalListener<StaminaSignalInvoker.AdrenalineGainedUnclampedSignal>
{
    [SerializeField] private AdrenalineGainEffect _vfx;
    [SerializeField] private AnimationCurve _amountToSize;

    protected override void OnSignal(StaminaSignalInvoker.AdrenalineGainedUnclampedSignal signal)
    {
        Debug.Log(signal.Value);
        var effect = Instantiate(_vfx, transform);
        var size = _amountToSize.Evaluate(signal.Value);
        effect.SetSize(size);
    }
}
