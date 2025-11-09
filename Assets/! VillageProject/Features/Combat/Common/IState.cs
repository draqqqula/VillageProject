using System;
using System.Collections.Generic;

public interface IState : IDisposable
{
    public StateType StateType { get; }
    public void OnEnter();
    public void OnExit();
    public IEnumerable<ITransition> GetTransitions();
}

public enum StateType
{
    Idle,
    Slash,
    Swing,
    Thrust,
    Guard
}