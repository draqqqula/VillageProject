using R3;
using System.Collections;
using UnityEngine;

public abstract class EndingState<T> : StateBase<T> where T : StateBase<T>
{
    protected ReactiveProperty<bool> _hasEnded { get; set; } = new ReactiveProperty<bool>(false);
    public ReadOnlyReactiveProperty<bool> HasEnded => _hasEnded;
}