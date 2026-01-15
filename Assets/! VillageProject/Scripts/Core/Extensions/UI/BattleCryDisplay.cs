using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleCryDisplay : SignalListener<BattleCrySignalInvoker.BattleCryStartedSignal, BattleCrySignalInvoker.BattleCryStartCooldownSignal>
{
    [SerializeField] private Image _image;
    private Coroutine _coroutine;

    protected override void OnSignal(BattleCrySignalInvoker.BattleCryStartedSignal signal)
    {
        ReleaseDisplay();
    }
    
    protected override void OnSignal(BattleCrySignalInvoker.BattleCryStartCooldownSignal signal)
    {
        StartCooldownFilling(signal.Cooldown);
    }

    private void ReleaseDisplay()
    {
        _image.fillAmount = 0;
    }
    
    private void StartCooldownFilling(float cooldown)
    {
        _image.fillAmount = 0;
        
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(CooldownRoutine(cooldown));
    }

    private IEnumerator CooldownRoutine(float cooldown)
    {
        var currentTime = 0f;
        
        while (currentTime < cooldown)
        {
            currentTime += Time.deltaTime;
            _image.fillAmount = currentTime / cooldown;
            yield return null;
        }
        _image.fillAmount = 1;
        _coroutine = null;
    }
}
