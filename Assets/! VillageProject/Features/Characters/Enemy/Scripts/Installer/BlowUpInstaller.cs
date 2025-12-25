using UnityEngine;
using Zenject;

public class BlowUpInstaller : MonoInstaller
{
    [SerializeField] private BlowUp _blowUp;
    
    public override void InstallBindings()
    {
        Container.Bind<BlowUp>().FromInstance(_blowUp).AsSingle();
    }
}
