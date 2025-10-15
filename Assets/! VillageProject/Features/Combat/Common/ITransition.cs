using R3;
using System;

public interface ITransition : IDisposable
{
    public Observable<IState> OnActivated { get; }
}