using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [SerializeField] private Transform _originPoint;
    
    private IndicatorController _indicatorController;
    public Origin Origin { get; private set; }

    [Inject]
    private void Construct(IndicatorController indicatorController)
    {
        _indicatorController = indicatorController;
    }
    
    public override void InstallBindings()
    {
        BindOrigin();
    }

    private void BindOrigin()
    {
        var deathEvent = GetComponent<DeathEvent>();
        Origin = new Origin(_originPoint, deathEvent, _indicatorController);
        Container.Bind<Origin>().FromInstance(Origin).AsSingle();
    }
}
