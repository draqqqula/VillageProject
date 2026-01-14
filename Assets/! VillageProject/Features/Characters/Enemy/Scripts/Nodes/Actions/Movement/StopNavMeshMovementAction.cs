using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Stop NavMesh Movement", story: "[Agent] stop NavMesh movement", category: "Action", id: "fdadf68bc4206dc9c16c48d2ff597100")]
public partial class StopNavMeshMovementAction : Action
{
    [SerializeReference] public BlackboardVariable<NavmeshMovementAgent> Agent;
    protected override Status OnStart()
    {
        Agent.Value.StopAgent();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }
}