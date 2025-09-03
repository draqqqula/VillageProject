using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using BezierSolution;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Go To Position Navmesh", story: "[Agent] goes to [Destination] using Navmesh", category: "Action/Movement", id: "d44622aabd2f278a743426ae3507aeed")]
public partial class GoToPositionNavmeshAction : StartMovementActionBase<NavmeshMovementAgent, Vector3>
{
    [SerializeReference] public BlackboardVariable<NavmeshMovementAgent> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Destination;
    public override NavmeshMovementAgent InstructionsAcceptor => Agent.Value;
    public override Vector3 Instructions => Destination.Value;
}

