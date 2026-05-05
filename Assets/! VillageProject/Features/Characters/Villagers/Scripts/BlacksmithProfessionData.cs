using UnityEngine;

[CreateAssetMenu(fileName = "BlacksmithProfession", menuName = "Villagers Simulation/Professions/New Blacksmith Profession")]
public class BlacksmithProfessionData : ProfessionData
{
    [field: SerializeField] public TowerAmmunition RaisingAmmunition {get; private set;}
    [field: SerializeField] public AnimationCurve RaiseHoursForExperienceCurve {get; private set;}
    [field: SerializeField] public float ExperienceForRaisingAmmunition {get; private set;}
}

