using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static NightInfo;

public class WaveController : MonoBehaviour
{
    public UnityEvent AllWavesCompleted;
    public UnityEvent<int> OnWaveStarted;
    [SerializeField] private NightInfo _night;
    [SerializeField] private List<EnemySpawner> _spawners;
    private bool _spawnComplete;
    private List<Health> Units;
    private int _spawnsRemaining;
    private int _unitsRemaining;
    private bool _isSpawning;
    private IEnumerator<(WaveWithPreparaion, int)> _wavesSequence;

    private void Awake()
    {
        foreach (var spawner in _spawners)
        {
            spawner.OnUnitSpawned += HandleUnitSpawned;
        }
    }

    private void Start()
    {
        _wavesSequence = _night.Waves.Select((it, i) => (it, i + 1)).GetEnumerator();
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        if (!_wavesSequence.MoveNext())
        {
            AllWavesCompleted?.Invoke();
            yield break;
        }

        yield return new WaitForSeconds(_wavesSequence.Current.Item1.PreparationTime);

        OnWaveStarted?.Invoke(_wavesSequence.Current.Item2);
        var wave = _wavesSequence.Current.Item1.Wave;
        _unitsRemaining = 0;
        _spawnsRemaining = wave.Spawns.Count;
        _isSpawning = true;
        foreach (var spawn in wave.Spawns)
        {
            var spawner = _spawners[spawn.SpawnpointIndex];
            var source = spawner.Schedule(spawn);
            source.OnFinished += CloseSpawn;
        }
    }

    private void CloseSpawn(WorkResult workResult)
    {
        _spawnsRemaining -= 1;
        if (_spawnsRemaining == 0)
        {
            _isSpawning = false;
        }
    }

    private void HandleUnitSpawned(GameObject unit)
    {
        _unitsRemaining++;
        var deathEvent = unit.GetComponent<DeathEvent>();
        if (deathEvent != null)
        {
            deathEvent.Fired.AddListener(HandleUnitDied);
        }
    }

    private IEnumerator StartNextWaveAfterCooldown()
    {
        yield return new WaitForSeconds(_wavesSequence.Current.Item1.CooldownTime);
        StartCoroutine(StartNextWave());
    }

    public void HandleUnitDied()
    {
        _unitsRemaining--;
        if (_unitsRemaining == 0 && !_isSpawning)
        {
            StartCoroutine(StartNextWaveAfterCooldown());
        }
    }
}
