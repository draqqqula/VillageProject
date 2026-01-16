using R3;
using UnityEngine;
using Zenject;

public class RestoreHealthAfterWave : MonoBehaviour
{
    [Inject] private Health _health;
    [Inject(Optional = true)] private WaveController _waveController;

    private void Start()
    {
        if (_waveController != null)
        {
            _waveController.IsOnBreak.Subscribe(HandleBreak).AddTo(this);
        }
    }

    private void HandleBreak(bool value)
    {
        if (true)
        {
            _health.Amount = _health.MaxHealth;
        }
    }
}
