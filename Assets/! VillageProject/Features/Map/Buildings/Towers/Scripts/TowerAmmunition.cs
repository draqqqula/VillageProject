using System.Collections;
using UnityEngine;

public class TowerAmmunition : ScriptableObject
{
    [SerializeReference, SubclassSelector] public ProjectileSpawner Spawner;
    [field: SerializeField] public uint MaxAmount { get; private set; }
    [field: SerializeField] public ResourceVariable Resource { get; private set; }
}