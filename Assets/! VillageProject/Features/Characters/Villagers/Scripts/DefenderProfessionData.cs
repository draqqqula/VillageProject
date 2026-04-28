using UnityEngine;

[CreateAssetMenu(fileName = "DefenderProfession", menuName = "Villagers Simulation/Professions/New Defender Profession")]
public class DefenderProfessionData : ProfessionData
{
    [field: SerializeField] public AnimationCurve DamageMultiplierCurve {get; private set;}
}