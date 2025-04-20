using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Team", menuName = "Team")]
public class Team : ScriptableObject
{
    [field: SerializeField] public List<Team> Enemies { get; private set; } = new List<Team>();

    public bool IsEnemiesWith(Team other)
    {
        return Enemies.Contains(other);
    }
}
