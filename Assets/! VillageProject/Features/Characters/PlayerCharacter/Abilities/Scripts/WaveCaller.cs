using System;
using System.Collections;
using R3;
using UnityEngine;
using Zenject;

public class WaveCaller : MonoBehaviour
{
    [SerializeField] private float _callingDuration;
    
    private IStartWaveInput _startWaveInput;
    private SkipTimeController _skipTimeController;
    private WaveController _waveController;
    private Coroutine _coroutine;
    
    [SerializeField] private WaveCallView _waveCallView;
    private GameTimer _gameTimer;

    private bool _isBlocked = false;

    [Inject]
    private void Construct(IStartWaveInput startWaveInput, SkipTimeController skipTimeController, WaveController waveController,
        GameTimer gameTimer)
    {
        _startWaveInput = startWaveInput;
        _startWaveInput.IsHolding.Subscribe(CallWave).AddTo(this);
        
        _skipTimeController = skipTimeController;
        _waveController = waveController;
        _waveController.IsOnBreak.Subscribe(OnWaveStateChanged).AddTo(this);
        
        _gameTimer = gameTimer;
        _gameTimer.OnTick += OnTick;
    }
    
    private void OnTick(int currentTick)
    {
        if (!gameObject.activeInHierarchy) return;
        
        _skipTimeController.GetSkipTime(out var skipHour,  out var skipMinutes);
        
        if (_gameTimer.CurrentHour == skipHour && _gameTimer.CurrentMinute >= skipMinutes)
        {
            BlockWaveCaller();
        }
    }

    private void OnWaveStateChanged(bool isEndedWave)
    {
        if (isEndedWave) UnblockWaveCaller();
        else BlockWaveCaller();
    }

    private void CallWave(bool isHolding)
    {
        if (_isBlocked) return;
        
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
    
    private void BlockWaveCaller()
    {
        if (_isBlocked) return;
        
        if (_coroutine != null) StopCoroutine(_coroutine);
        _waveCallView.UpdateProgress(0);
        
        _waveCallView.DeactivateView();
        _isBlocked = true;
    }

    private void UnblockWaveCaller()
    {
        if (!_isBlocked) return;
        
        _waveCallView.ActivateView();
        _isBlocked = false;
    }

    private void OnDestroy()
    {
        _gameTimer.OnTick -= OnTick;
    }
}