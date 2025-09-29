using System.Collections;
using UnityEngine;
using Zenject;

public class DefeatOnPlayerDeath : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private MatchState _matchState;

    private void Awake()
    {
        _signalBus.Subscribe<PlayerHealthSignalInvoker.PlayerDeathSignal>(it => _matchState.DeclareDefeat());
    }
}