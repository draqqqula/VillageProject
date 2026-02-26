using System;
using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public class WaveCaller : MonoBehaviour
{
    [SerializeField] private float _callingDuration;
    
    private IStartWaveInput _startWaveInput;
    [SerializeField] private SkipTimeController _skipTimeController;
    [SerializeField] private WaveController _waveController;
    private Coroutine _coroutine;
    
    [SerializeField] private WaveCallView _waveCallView;

    [Inject]
    private void Construct(IStartWaveInput startWaveInput)
    {
        _startWaveInput = startWaveInput;
        _startWaveInput.IsHolding.Subscribe(CallWave).AddTo(this);
    }

    private void CallWave(bool isHolding)
    {
        if (!_waveController.IsOnBreak.CurrentValue) return;
        
        if (isHolding)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(StartCalling());
        }
        else 
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            _waveCallView.UpdateProgress(0);
        }
    }

    private IEnumerator StartCalling()
    {
        var currentTime = 0f;

        while (currentTime < _callingDuration)
        {
            currentTime += Time.deltaTime;
            _waveCallView.UpdateProgress(currentTime / _callingDuration);
            yield return null;
        }
        
        _skipTimeController.SkipToNextWave();
        _waveCallView.UpdateProgress(0);
        _waveCallView.gameObject.SetActive(false);
    }
}