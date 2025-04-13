using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Assign Animator", story: "Sets [Animator] Value from [Root] children", category: "Action/Find/Assign", id: "d626d629e4bdd7129688574c24919a70")]
public partial class AssignAnimatorAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Root;
    [SerializeReference] public BlackboardVariable<Animator> Animator;

    protected override Status OnStart()
    {
        Animator.Value = Root.Value.GetComponentInChildren<Animator>();
        return Animator.Value == null ? Status.Failure : Status.Success;
    }
}

