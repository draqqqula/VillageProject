using R3;
using System;
using System.Collections;
using UnityEngine;

public interface IAnimationWindowListener
{
    public event Action OnEnter;
    public event Action OnExit;
    public float Progress { get; set; }
    public ReadOnlyReactiveProperty<bool> IsActive { get; }
}