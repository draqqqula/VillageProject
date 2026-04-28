using UnityEngine;

[CreateAssetMenu(fileName = "ArmorerProfession", menuName = "Villagers Simulation/Professions/New Armorer Profession")]
public class ArmorerProfessionData : ProfessionData
{
    [field: SerializeField] public AnimationCurve HealthMultiplierCurve {get; private set;}
}