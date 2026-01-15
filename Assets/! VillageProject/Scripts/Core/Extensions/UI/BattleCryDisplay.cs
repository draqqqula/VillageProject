using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleCryDisplay : SignalListener<BattleCrySignalInvoker.BattleCryStartedSignal, BattleCrySignalInvoker.BattleCryCooldownProgressSignal>
{
    [SerializeField] private Image _image;
    private Coroutine _coroutine;

    protected override void OnSignal(BattleCrySignalInvoker.BattleCryStartedSignal signal)
    {
        _image.fillAmount = 0;
    }
    
    protected override void OnSignal(BattleCrySignalInvoker.BattleCryCooldownProgressSignal progressSignal)
    {
        _image.fillAmount = progressSignal.Progress;
    }
}
