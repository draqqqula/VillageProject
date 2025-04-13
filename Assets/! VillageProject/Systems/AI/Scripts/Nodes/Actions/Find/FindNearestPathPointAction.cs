using BezierSolution;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Nearest Path Point", story: "[Agent] searches for nearest [Position] on [Path]", category: "Action/Find", id: "cf4cb36849e24549bcb76016eed12bd9")]
public partial class FindNearestPathPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<BezierSpline> Path;
    [SerializeReference] public BlackboardVariable<Vector3> Position;

    protected override Status OnStart()
    {
        Position.Value = Path.Value.FindNearestPointTo(Agent.Value.transform.position);
        return Status.Success;
    }
}

