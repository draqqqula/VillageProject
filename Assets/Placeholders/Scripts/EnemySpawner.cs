using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    public Action<GameObject> OnUnitSpawned;
    [Inject] DiContainer _container;
    [SerializeField] private float _aiDelay;
    [SerializeField] private GameObject _village;
    [SerializeField] private GameObject _road;
    [SerializeField] private GameObject _player;

    private IEnumerator SpawnOnInterval(WaveInfo.Spawn spawn, WorkEventSource eventSource)
    {
        foreach (var group in spawn.Groups)
        {
            yield return new WaitForSeconds(group.RelaxTime);
            for (var i = 0; i < group.Amount; i++)
            {
                var unit = Spawn(group.Unit);
                yield return new WaitForSeconds(group.Interval);
            }
        }
        eventSource.Finish(WorkResult.Success);
    }

    private IEnumerator DelayAI(BehaviorGraphAgent agent)
    {
        yield return new WaitForSeconds(_aiDelay);
        agent.enabled = true;
    }

    protected virtual GameObject Spawn(GameObject unit, Transform transform = null)
    {
        if (transform == null) transform = this.transform;
        
        var enemy = _container.InstantiatePrefab(unit, transform.position, transform.rotation, null);
        var ai = enemy.GetComponent<BehaviorGraphAgent>();
        ai.enabled = false;
        StartCoroutine(DelayAI(ai));
        ai.SetVariableValue("VillageZone", _village);
        ai.SetVariableValue("RoadZone", _road);
        ai.SetVariableValue("Player", _player);
        OnUnitSpawned?.Invoke(enemy);

        return enemy;
    }

    public IWorkEventSource<WorkResult> Schedule(WaveInfo.Spawn spawn)
    {
        var eventSource = new WorkEventSource();
        StartCoroutine(SpawnOnInterval(spawn, eventSource));
        return eventSource;
    }
}
