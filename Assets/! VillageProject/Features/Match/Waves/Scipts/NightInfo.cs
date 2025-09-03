using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Night", menuName = "Night")]
public class NightInfo : ScriptableObject
{

    [Serializable]
    public class WaveWithPreparaion
    {
        [field: SerializeField] public float PreparationTime { get; private set; }
        [field: SerializeField] public WaveInfo Wave { get; private set; }
        [field: SerializeField] public float CooldownTime { get; private set; }
    }

    [Serializable]
    public class Target
    {
        [field: SerializeField] public float Health { get; private set; }
    }

    [SerializeField] private List<WaveWithPreparaion> _waves;
    [SerializeField] private List<Target> _targets;
    public IEnumerable<WaveWithPreparaion> Waves => _waves;
    public IEnumerable<Target> Targets => _targets;
}