using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayOnEnter : StateMachineBehaviour
{
    [SerializeField] private SwordCombatSounds _sound;
    [Inject] private SignalBus _signalBus;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _signalBus.Fire(new PlayAudioSignal<SwordCombatSounds>(_sound));
    }
}