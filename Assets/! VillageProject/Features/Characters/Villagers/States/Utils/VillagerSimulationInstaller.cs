using UnityEngine;
using Zenject;

public class VillagerSimulationInstaller : MonoInstaller<VillagerSimulationInstaller>
{
    [SerializeField] private VillagerSystem _villagerSystem;
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private SkipTimeController _skipTimeController;
    [SerializeField] private Transform _villageCenter;
    [SerializeField] private Transform _villagersSpawnPoint;
    [SerializeField] private ProfessionController _professionController;
    [SerializeField] private InterestingPointsService _interestingPointsService;
    
    [SerializeField] private UpgradeMenu _swordUpgrader;
    [SerializeField] private UpgradeMenu _armorUpgrader;

    [SerializeField] private SkipView _skipUI;

    public override void InstallBindings()
    {
        Container.BindInstance(_villagerSystem).AsSingle();
        Container.BindInstance(_gameTimer).AsSingle();
        Container.BindInstance(_skipUI).AsSingle();
        Container.BindInstance(_skipTimeController).AsSingle();
        Container.BindInstance(_interestingPointsService).AsSingle();
        
        Container.BindInstance(_villageCenter).WithId("VillageCenter").AsCached();
        Container.BindInstance(_villagersSpawnPoint).WithId("VillagersSpawnPoint").AsCached();
        Container.BindInstance(_professionController).AsSingle();
        
        Container.BindInstance(_swordUpgrader).WithId("UpgradeSword").AsCached();
        Container.BindInstance(_armorUpgrader).WithId("UpgradeArmor").AsCached(); 
    }
}