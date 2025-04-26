using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Syncronize Blackboard", story: "Log variable [Variable]", category: "Action", id: "93d58c8f0b5016b458b53f672e4df405")]
public partial class SyncronizeBlackboardAction : Action
{
    [SerializeReference] public BlackboardVariable<Target> Variable;

    protected override Status OnStart()
    {
        Debug.Log(Variable.Value.gameObject.name);
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

