using UnityEngine;
using Zenject;

public class VillagersSimulationBootstrap : MonoBehaviour
{
    [SerializeField] private VillagerSystem _villagerSystem;
    [SerializeField] private ScheduleController _scheduleController;
    [SerializeField] private ProfessionController _professionController;
    [SerializeField] private LoyaltyController _loyaltyController;
    [SerializeField] private ScheduleParamsHandler _scheduleParamsHandler;
    
    [Inject] private BuildingPlanner _buildingPlanner;

    private void Start()
    {
        _buildingPlanner.Init();
        _villagerSystem.Init();
        _scheduleController.Init();
        _professionController.Init();
        _loyaltyController.Init();
        _scheduleParamsHandler.Init(_scheduleController, _villagerSystem);
    }
}