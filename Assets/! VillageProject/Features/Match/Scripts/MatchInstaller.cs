using System.Collections;
using UnityEngine;
using Zenject;

public class MatchInstaller : MonoInstaller
{
    [SerializeField] private MatchState _matchState;
    public override void InstallBindings()
    {
        Container.BindInstance(_matchState).AsSingle();
    }
}