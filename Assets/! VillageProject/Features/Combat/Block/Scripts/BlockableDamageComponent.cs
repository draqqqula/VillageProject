using UnityEngine;
using System;
using UnityEngine.Serialization;
using Zenject;

[Serializable]
public class BlockableDamageComponent : DamageComponentBase
{
    [SerializeField] private StateOfAttack stateOfAttack;
    public bool IsBlocking => stateOfAttack.IsBlocking;

    public void Apply()
    {
        stateOfAttack.IsBlocking = true;
    }
}