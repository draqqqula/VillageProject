using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using UnityEngine.Events;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Target Detected State", story: "[Agent] spotted [Target]", category: "Flow", id: "97c8adba28f577a96e3598a12cfbaeab")]
public partial class TargetDetectedStateSequence : BinaryStateSequenceBase
{
    [SerializeReference] public BlackboardVariable<SearchForTarget> Agent;
    [SerializeReference] public BlackboardVariable<Target> Target;

    protected override bool GetValue()
    {
        if (Agent.Value.MainTarget != null)
        {
            Target.Value = Agent.Value.MainTarget;
            return true;
        }
        return false;
    }

    protected override void Subscribe(System.Action handler)
    {
        Agent.Value.OnMainTargetChanged.AddListener(new UnityAction(handler));
    }

    protected override void Unsubscribe(System.Action handler)
    {
        Agent.Value.OnMainTargetChanged.RemoveListener(new UnityAction(handler));
    }
}

