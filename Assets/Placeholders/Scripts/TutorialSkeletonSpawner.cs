using System.Collections;
using UnityEngine;
using Zenject;

public class TutorialSkeletonSpawner : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private SceneContext _context;

    public void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        var go = _context.Container.InstantiatePrefab(_prefab, transform);
        var death = go.GetComponent<DeathEvent>();
        death.FiredEvent += SpawnWithDelay;
    }

    public void SpawnWithDelay()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnAfterDelay());
    }

    public IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(_delay);
        Spawn();
    }
}
