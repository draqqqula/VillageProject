using UnityEngine;

[CreateAssetMenu(fileName = "BuilderProfession", menuName = "Villagers Simulation/Professions/New Builder Profession")]
public class BuilderProfessionData : ProfessionData
{
    [field: SerializeField] public AnimationCurve PlanDurationMultiplierCurve {get; private set;}
}