using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _interval;
    [SerializeField] private float _aiDelay;
    [SerializeField] private int _remaining;
    [SerializeField] private GameObject _enemy;
    [SerializeField] private GameObject _village;
    [SerializeField] private GameObject _road;
    [SerializeField] private GameObject _player;

    private void Start()
    {
        StartCoroutine(SpawnOnInterval());
    }

    private IEnumerator SpawnOnInterval()
    {
        while (_remaining > 0)
        {
            yield return new WaitForSeconds(_interval);
            Spawn();
        }
    }

    private IEnumerator DelayAI(BehaviorGraphAgent agent)
    {
        yield return new WaitForSeconds(_aiDelay);
        agent.enabled = true;
    }

    private void Spawn()
    {
        _remaining--;
        var enemy = Instantiate(_enemy, transform.position, transform.rotation);
        var ai = enemy.GetComponent<BehaviorGraphAgent>();
        ai.enabled = false;
        StartCoroutine(DelayAI(ai));
        ai.SetVariableValue("VillageZone", _village);
        ai.SetVariableValue("RoadZone", _road);
        ai.SetVariableValue("Player", _player);
    }
}
