using System.Linq;
using R3;
using UnityEngine;

public class WaveStarter : MonoBehaviour
{
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private int[] _startWaveHours;
    [SerializeField] private WaveController _waveController;

    private void Awake()
    {
        _gameTimer.OnHourChanged += OnHourChanged;
    }

    private void OnHourChanged(int hour)
    {
        if (_startWaveHours.Any(h => hour == h)) StartWave();
    }

    private void StartWave()
    {
        _gameTimer.Pause();
        _waveController.FinishBreak();
        _waveController.IsOnBreak.Subscribe(OnBreakChanged).AddTo(this);
    }

    private void OnBreakChanged(bool isOnBreak)
    {
        if (isOnBreak) FinishWave();
    }

    private void FinishWave()
    {
        _gameTimer.Resume(); 
    }
}