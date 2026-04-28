using UnityEngine;

[CreateAssetMenu(fileName = "ArcherProfession", menuName = "Villagers Simulation/Professions/New Archer Profession")]
public class ArcherProfessionData : ProfessionData
{
    [field: SerializeField] public AnimationCurve DamageMultiplierCurve {get; private set;}
}