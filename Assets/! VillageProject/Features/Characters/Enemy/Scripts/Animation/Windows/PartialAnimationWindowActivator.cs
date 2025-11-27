using System.Collections;
using UnityEngine;
using Zenject;

public class PartialAnimationWindowActivator : StateMachineBehaviour
{
    [SerializeField] private AnimationWindow _window;
    [SerializeField, Range(0, 1)] private float _startNormalized;
    [SerializeField, Range(0, 1)] private float _endNormalized;
    private IAnimationWindowController _controller;

    [Inject]
    private void Construct(DiContainer container)
    {
        _controller = container.ResolveId<IAnimationWindowController>(_window);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var actual = stateInfo.normalizedTime >= _startNormalized && stateInfo.normalizedTime <= _endNormalized;
        if (actual != _controller.Active)
        {
            _controller.Active = actual;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    private void OnValidate()
    {
        if (_endNormalized < _startNormalized)
        {
            _endNormalized = _startNormalized;
        }
    }
}