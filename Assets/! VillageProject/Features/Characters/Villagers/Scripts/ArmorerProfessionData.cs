using UnityEngine;

[CreateAssetMenu(fileName = "ArmorerProfession", menuName = "Villagers Simulation/New Armorer Profession")]
public class ArmorerProfessionData : ProfessionData
{
    [field: SerializeField] public AnimationCurve HealthMultiplierCurve {get; private set;}
}