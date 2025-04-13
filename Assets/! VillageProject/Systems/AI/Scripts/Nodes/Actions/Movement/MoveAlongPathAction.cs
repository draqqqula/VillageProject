using BezierSolution;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move Along Path", story: "[Agent] starts walking [Path]", category: "Action/Movement", id: "e18d249009c28b1c4ea4f6e26dbeb292")]
public partial class MoveAlongPathAction : StartMovementActionBase<BezierCurveMovementAgent, BezierSpline>
{
    [SerializeReference] public BlackboardVariable<BezierCurveMovementAgent> Agent;
    [SerializeReference] public BlackboardVariable<BezierSpline> Path;
    public override BezierCurveMovementAgent InstructionsAcceptor => Agent.Value;
    public override BezierSpline Instructions => Path.Value;

    protected override void OnEnd()
    {
        base.OnEnd();
    }
}

