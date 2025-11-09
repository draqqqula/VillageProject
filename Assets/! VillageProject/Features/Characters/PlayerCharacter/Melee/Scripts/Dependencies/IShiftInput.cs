using R3;
using System.Collections;
using UnityEngine;

public interface IShiftInput
{
    public ReadOnlyReactiveProperty<bool> IsHolding { get; }
}