using UnityEngine;
using Zenject;

public class VillagerSimulationInstaller : MonoInstaller<VillagerSimulationInstaller>
{
    [SerializeField] private VillagerSystem _villagerSystem;
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private Transform _villageCenter;
    [SerializeField] private ProfessionController _professionController;

    public override void InstallBindings()
    {
        Container.BindInstance(_villagerSystem).AsSingle();
        Container.BindInstance(_gameTimer).AsSingle();
        Container.BindInstance(_villageCenter).WithId("VillageCenter").AsSingle();
        Container.BindInstance(_professionController).AsSingle();
    }
}