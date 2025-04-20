using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class WaveController : MonoBehaviour
{
    public UnityEvent WaveComplete;
    [SerializeField] private WaveInfo _wave;
    [SerializeField] private List<EnemySpawner> _spawners;
    private bool _spawnComplete;
    private List<Health> Units;
    private int _spawnsRemaining;
    private int _unitsRemaining;
    private bool _isSpawning;

    private void Awake()
    {
        foreach (var spawner in _spawners)
        {
            spawner.OnUnitSpawned += HandleUnitSpawned;
        }
    }

    private void Start()
    {
        _unitsRemaining = 0;
        _spawnsRemaining = _wave.Spawns.Count;
        _isSpawning = true;
        foreach (var spawn in _wave.Spawns)
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

    public void HandleUnitDied()
    {
        _unitsRemaining--;
        if (_unitsRemaining == 0 && !_isSpawning)
        {
            WaveComplete?.Invoke();
        }
    }
}
