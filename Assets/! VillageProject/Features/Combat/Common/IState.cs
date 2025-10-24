using System;
using System.Collections.Generic;

public interface IState : IDisposable
{
    public void OnEnter();
    public void OnExit();
    public IEnumerable<ITransition> GetTransitions();
}
