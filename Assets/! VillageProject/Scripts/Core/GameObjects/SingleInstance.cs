using System;
using UnityEngine;
using Zenject;

public class SingleInstance : MonoBehaviour
{
    public event Action InstanceChanged;
    [field: SerializeField] public GameObject Instance { get; private set; }

    [Inject]
    private void Construct(BuildingStorage buildingStorage)
    {
        if (Instance.TryGetComponent(out Building building))
        {
            buildingStorage.Add(building);
        }
    }
    
    public void Substitute(GameObject newer)
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = newer;
        InstanceChanged?.Invoke();
    }
}
