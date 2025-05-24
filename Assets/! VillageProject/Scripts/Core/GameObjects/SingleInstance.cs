using System;
using UnityEngine;

public class SingleInstance : MonoBehaviour
{
    public event Action InstanceChanged;
    [field: SerializeField] public GameObject Instance { get; private set; }

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
