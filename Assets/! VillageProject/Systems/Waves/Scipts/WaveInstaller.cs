using System.Collections;
using UnityEngine;
using Zenject;

public class WaveInstaller : MonoInstaller
{
    [SerializeField] private WaveController _waveController;
    [SerializeField] private ShoppingZone _shoppingZone;
    [SerializeField] private MatchObjective _matchObjective;

    public override void InstallBindings()
    {
        Container.BindInstance(_waveController).AsSingle();
        Container.BindInstance(_shoppingZone).AsSingle();
        Container.BindInstance(_matchObjective).AsSingle();
    }

    public override void Start()
    {
        base.Start();
        _matchObjective.Initialize(1, 1f, 1f);
    }
}
