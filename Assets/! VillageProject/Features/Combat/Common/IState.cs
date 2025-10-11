using System.Collections.Generic;

public interface IState
{
    public void OnEnter();
    public void OnExit();
    public IEnumerable<ITransition> GetTransitions();
}
