using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Blowing Up State", story: "[Self] is blowing up", category: "Flow", id: "570a26497aa87babdd08b9ef58b0b1cb")]
public partial class BlowingUpStateSequence : BinaryStateSequenceBase
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    
    protected override bool GetValue()
    {
        if (Self.Value.TryGetComponent<BlowUp>(out var blowUp))
        {
            return blowUp.IsBlowing;
        }
        return false;
    }

    protected override void Subscribe(System.Action handler)
    {
        if (Self.Value.TryGetComponent<BlowUp>(out var blowUp))
        {
            blowUp.OnStarted += handler;
        }
    }

    protected override void Unsubscribe(System.Action handler)
    {
        if (Self.Value.TryGetComponent<BlowUp>(out var blowUp))
        {
            blowUp.OnStarted -= handler;
        }
    }
}

