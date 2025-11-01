using System.Collections;
using UnityEngine;
using Zenject;

public class ThrustState : AnimationState<ThrustState>
{
    private const string ThrustTrigger = "Thrust";
    public override StateType StateType => StateType.Thrust;

    [Inject] private Animator _animator;
    [Inject] private SlashConfiguration _slashConfiguration;
    [Inject] private SwingConfiguration _swingConfiguration;
    [Inject] private Stamina _stamina;

    protected override AnimationWindow Window => _slashConfiguration.ThrustWindow;

    public override void OnEnter()
    {
        base.OnEnter();
        _stamina.TrySpend(_swingConfiguration.StaminaCost);
        _animator.SetTrigger(ThrustTrigger);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}