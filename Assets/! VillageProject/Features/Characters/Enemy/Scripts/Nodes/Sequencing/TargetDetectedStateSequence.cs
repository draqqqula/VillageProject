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
        //if (True.CurrentStatus != Status.Uninitialized)
        //{
        //    EndNode(True);
        //}
        //else if (False.CurrentStatus != Status.Uninitialized)
        //{
        //    EndNode(False);
        //}
        Target.Value = Agent.Value.MainTarget;
        return Agent.Value.MainTarget != null;
    }

    protected override void Subscribe(System.Action handler)
    {
        Agent.Value.OnMainTargetChangedEvent += handler;
    }

    protected override void Unsubscribe(System.Action handler)
    {
        Agent.Value.OnMainTargetChangedEvent -= handler;
    }
}

