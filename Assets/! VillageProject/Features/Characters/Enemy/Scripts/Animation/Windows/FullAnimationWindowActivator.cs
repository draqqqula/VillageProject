using UnityEngine;
using Zenject;

public class FullAnimationWindowActivator : StateMachineBehaviour
{
    [SerializeField] private AnimationWindow _window;
    private IAnimationWindowController _controller;

    [Inject]
    private void Construct(DiContainer container)
    {
        _controller = container.ResolveId<IAnimationWindowController>(_window);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller.Active = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _controller.Active = false;
    }
}
