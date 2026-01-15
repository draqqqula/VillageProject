using UnityEngine;
using UnityEngine.Animations;
using Zenject;

public class PlayOnTiming : StateMachineBehaviour
{
        [SerializeField, Range(0, 1)] private float _normilizedT;
        [SerializeField] private SwordCombatSounds _sound;
        [Inject] private SignalBus _signalBus;
        private bool _played = false;
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
                if (!_played && stateInfo.normalizedTime >= _normilizedT)
                {
                        _signalBus.Fire(new PlayAudioSignal<SwordCombatSounds>(_sound));
                        _played = true;
                }
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
                _played = false;
        }
}