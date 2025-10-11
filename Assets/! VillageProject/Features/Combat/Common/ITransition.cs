using R3;
using System;

public interface ITransition
{
    public Observable<Unit> OnActivated { get; }
    public IState GetNextState();
}