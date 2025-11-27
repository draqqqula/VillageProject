using R3;
using System;
using System.Collections;
using UnityEngine;

public interface IAnimationWindowListener
{
    public event Action OnEnter;
    public event Action OnExit;
    public ReadOnlyReactiveProperty<bool> IsActive { get; }
}