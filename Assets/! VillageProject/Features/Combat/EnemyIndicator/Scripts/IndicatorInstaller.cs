using UnityEngine;
using Zenject;

public class IndicatorInstaller : MonoInstaller
{
    [SerializeField] private IndicatorController _indicatorController;
    
    public override void InstallBindings()
    {
        Container.Bind<IndicatorController>().FromInstance(_indicatorController).AsSingle();
    }
}
