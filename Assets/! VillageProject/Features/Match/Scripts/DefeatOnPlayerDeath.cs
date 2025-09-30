using System.Collections;
using UnityEngine;
using Zenject;

public class DefeatOnPlayerDeath : SignalListener<PlayerHealthSignalInvoker.PlayerDeathSignal>
{
    [Inject] private MatchState _matchState;

    protected override void OnSignal(PlayerHealthSignalInvoker.PlayerDeathSignal signal)
    {
        _matchState.DeclareDefeat();
    }
}