using R3;
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
    public UnityEvent BreakStarted;
    public UnityEvent BreakFinished;
    public ReadOnlyReactiveProperty<bool> IsOnBreak => _isOnBreak;

    [SerializeField] private NightInfo _night;
    [SerializeField] private List<EnemySpawner> _spawners;
    private bool _spawnComplete;
    private List<Health> Units;
    private int _spawnsRemaining;
    private int _unitsRemaining;
    private bool _isSpawning;
    private IEnumerator<(WaveWithPreparaion, int)> _wavesSequence;
    private ReactiveProperty<bool> _isOnBreak = new ReactiveProperty<bool>(false);

    public WaveInfo CurrentWave { get; private set; }
    public IReadOnlyList<EnemySpawner> Spawners => _spawners;

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
        ScheduleWave();
    }

    private void ScheduleWave()
    {
        if (!_wavesSequence.MoveNext())
        {
            CurrentWave = null;
            AllWavesCompleted?.Invoke();
            return;
        }
        CurrentWave = _wavesSequence.Current.Item1.Wave;
        _isOnBreak.Value = true;
        BreakStarted?.Invoke();
    }

    private void InvokeWave()
    {
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
        ScheduleWave();
    }

    public void HandleUnitDied()
    {
        _unitsRemaining--;
        if (_unitsRemaining == 0 && !_isSpawning)
        {
            StartCoroutine(StartNextWaveAfterCooldown());
        }
    }

    public void FinishBreak()
    {
        if (_isOnBreak.Value)
        {
            _isOnBreak.Value = false;
            BreakFinished?.Invoke();
            InvokeWave();
        }
    }
}
