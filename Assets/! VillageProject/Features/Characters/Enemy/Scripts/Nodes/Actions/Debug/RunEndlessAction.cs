using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Run Endless", story: "Always returns Running Status", category: "Action/Debug", id: "7fd3b35998888fe3f960b94f10ceb0ea")]
public partial class RunEndlessAction : Action
{

    protected override Status OnStart()
    {
        Debug.Log($"Endless Running node started");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Running;
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        Debug.Log($"Endless Running node ended with status: {CurrentStatus}");
    }
}

