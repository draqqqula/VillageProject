using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Stop Bezier Movement", story: "[Agent] stop Bezier movement", category: "Action/Movement", id: "071e1e82a23f9e4340d8d0cd66a43cbf")]
public partial class StopBezierMovementAction : Action
{
    [SerializeReference] public BlackboardVariable<BezierCurveMovementAgent> Agent;

    protected override Status OnStart()
    {
        Agent.Value.StopAgent();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

