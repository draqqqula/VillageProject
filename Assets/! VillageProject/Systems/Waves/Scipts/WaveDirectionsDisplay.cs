using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveDirectionDisplay : MonoBehaviour
{
    [SerializeField] private WaveController _waveController;
    [SerializeField] private List<GameObject> _displays;

    private void OnEnable()
    {
        _waveController.BreakStarted.AddListener(HandleWaveStarted);
    }

    private void OnDisable()
    {
        _waveController.BreakStarted.RemoveListener(HandleWaveStarted);
    }

    private void HandleWaveStarted()
    {
        foreach (var display in _displays)
        {
            display.SetActive(false);
        }
        foreach (var spawn in _waveController.CurrentWave.Spawns)
        {
            _displays[spawn.SpawnpointIndex].SetActive(true);
        }
    }
}
