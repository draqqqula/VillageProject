using UnityEngine;

[CreateAssetMenu(fileName = "BlacksmithProfession", menuName = "Villagers Simulation/Professions/New Blacksmith Profession")]
public class BlacksmithProfessionData : ProfessionData
{
    [field: SerializeField] public TowerAmmunition RaisingAmmunition {get; private set;}
    [field: SerializeField] public int HoursForRaisingAmmunition {get; private set;}
    [field: SerializeField] public float ExperienceForRaisingAmmunition {get; private set;}
}

