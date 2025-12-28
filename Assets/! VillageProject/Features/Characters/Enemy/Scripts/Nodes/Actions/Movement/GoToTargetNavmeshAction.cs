using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Go To Target Navmesh", story: "[Agent] follows [Target] using Navmesh", category: "Action/Movement", id: "b4cc9a4afb5cd6e526d6247cdcc0c7f9")]
public partial class GoToTargetNavmeshAction : StartMovementActionBase<NavmeshMovementAgent, GameObject>
{
    [SerializeReference] public BlackboardVariable<NavmeshMovementAgent> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    public override NavmeshMovementAgent InstructionsAcceptor => Agent;
    public override GameObject Instructions => Target;
}
