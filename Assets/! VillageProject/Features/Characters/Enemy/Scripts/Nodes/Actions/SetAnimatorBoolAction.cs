using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Animator Bool", story: "Sets animation bool [Bool] in [Animator] to: [BoolState]", category: "Action/Animation", id: "e2b6919fdea90423b78d1c79261f312d")]
public partial class SetAnimatorBoolAction : Action
{
    [SerializeReference] public BlackboardVariable<string> Bool;
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    [SerializeReference] public BlackboardVariable<bool> BoolState;

    protected override Status OnStart()
    {
        if (Animator.Value == null)
        {
            LogFailure("No Animator set.");
            return Status.Failure;
        }
        
        Animator.Value.SetBool(Bool.Value, BoolState.Value);
        
        return Status.Success;
    }
}

