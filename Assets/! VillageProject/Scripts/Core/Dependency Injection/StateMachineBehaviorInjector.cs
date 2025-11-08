using System.Collections;
using UnityEngine;
using Zenject;

public class StateMachineBehaviorInjector : MonoInstaller
{
    [SerializeField] private Animator _animator;
    public override void InstallBindings()
    {
        foreach (var behavior in _animator.GetBehaviours<StateMachineBehaviour>())
        {
            Debug.Log("queued for inject");
            Container.QueueForInject(behavior);
        }
    }
}