using R3;
using System.Collections;
using UnityEngine;
using Zenject;

public class SlashState : AnimationState<SlashState>
{
    public override StateType StateType => StateType.Slash;
    
    [Inject] private Stamina _stamina;
    [Inject] private SlashSeriesCounter _slashSeriesCounter;
    [Inject] private SlashConfiguration _slashConfiguration;
    [Inject] private SwingConfiguration _swingConfiguration;

    protected override AnimationWindow Window => _slashConfiguration.SlashWindow;
    public AttackDirection AttackDirection { get; set; }

    public override void OnEnter()
    {
        base.OnEnter();
        _stamina.TrySpend(_swingConfiguration.StaminaCost);
    }

    public override void OnExit()
    {
        base.OnExit();
        _slashSeriesCounter.SetNextAttack(AttackDirection);
    }
}