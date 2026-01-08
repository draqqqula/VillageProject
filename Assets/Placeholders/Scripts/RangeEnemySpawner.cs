using System;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SphereCollider))]
public sealed class RangeEnemySpawner : EnemySpawner
{
    private const float MinDistance = .7f;
    private const int MaxTries = 50;
    
    [SerializeField] private int _spawnPointCount;
    
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private GameObject _spawnPointPrefab;
    
    [SerializeField] private Transform[] _spawnPoints;
    
    protected override GameObject Spawn(GameObject unit, Transform transform = null)
    {
        var spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        return base.Spawn(unit, spawnPoint);
    }

    [ContextMenu("Generate Spawn Points")]
    private void GenerateSpawnPoints()
    {
        if (_spawnPoints != null)
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                if (spawnPoint != null) DestroyImmediate(spawnPoint.gameObject);
            }
        }
        
        _spawnPoints = new Transform[_spawnPointCount];
        for (var i = 0; i < _spawnPointCount; i++)
        {
            _spawnPoints[i] = GenerateSpawnPoint(i);
        }
    }

    private Transform GenerateSpawnPoint(int index)
    {
        var randomPoint = Instantiate(_spawnPointPrefab, transform);
        randomPoint.gameObject.name += $"_{index}";

        int tryCount = 0;
        bool isSucceded = false;
        do
        {
            if (tryCount > MaxTries)
            {
                DestroyImmediate(randomPoint.gameObject);
                throw new StackOverflowException("Spawn Point can't spawn!");
            }
            
            randomPoint.transform.position = transform.position + Random.insideUnitSphere * _collider.radius;
            isSucceded = TryMoveOnGround(randomPoint);
            tryCount++;
            
        } while (!isSucceded || IsNeedRespawn(randomPoint));
        
        return randomPoint.transform;
    }

    private bool TryMoveOnGround(GameObject randomPoint)
    {
        float rayDistance = 40;
        var rayOrigin = new Vector3(randomPoint.transform.position.x, transform.position.y + rayDistance / 2, randomPoint.transform.position.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Low"))
            {
                randomPoint.transform.position = hit.point;
                return true;
            }
        }
        return false;
    }

    private bool IsNeedRespawn(GameObject randomPoint)
    {
        return _spawnPoints.Any(point =>
                   point != null && Vector3.Distance(randomPoint.transform.position, point.position) < MinDistance);
    }
    
    private bool IsOnNavMesh(GameObject randomPoint)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint.transform.position, out hit, 0.1f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }
}
