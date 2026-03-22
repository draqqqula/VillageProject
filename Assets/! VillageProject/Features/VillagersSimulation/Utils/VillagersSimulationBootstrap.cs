using UnityEngine;

public class VillagersSimulationBootstrap : MonoBehaviour
{
    [SerializeField] private VillagerSystem _villagerSystem;
    [SerializeField] private ScheduleController _scheduleController;
    [SerializeField] private ProfessionController _professionController;

    private void Start()
    {
        _villagerSystem.Init();
        _scheduleController.Init();
        _professionController.Init();
    }
}