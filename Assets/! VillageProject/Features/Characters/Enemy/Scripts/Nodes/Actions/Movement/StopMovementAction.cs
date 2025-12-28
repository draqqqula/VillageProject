using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Stop Movement", story: "[Agent] stop movement", category: "Action/Movement", id: "fdadf68bc4206dc9c16c48d2ff597100")]
public partial class StopMovementAction : Action
{
    [SerializeReference] public BlackboardVariable<NavmeshMovementAgent> Agent;

    protected override Status OnStart()
    {
        if (Agent.Value.CurrentWork != null) Agent.Value.StopAgent();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }
}

