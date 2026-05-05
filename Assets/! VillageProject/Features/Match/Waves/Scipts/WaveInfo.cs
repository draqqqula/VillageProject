using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Wave")]
public class WaveInfo : ScriptableObject
{
    [Serializable]
    public class Group
    {
        public GameObject Unit;
        public int Amount;
        public float RelaxTime;
        public float Interval;
    }

    [Serializable]
    public class Spawn
    {
        public int SpawnpointIndex;
        public List<Group> Groups;
    }

    public List<Spawn> Spawns;
    [field: SerializeField, Range(0f, 1f)] public float Difficulty {get; private set;}
}
