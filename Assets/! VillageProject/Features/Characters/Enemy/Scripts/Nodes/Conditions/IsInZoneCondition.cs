using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is In Zone", story: "[Agent] is inside [Zone]", category: "Conditions", id: "123c3560fd40383338c122296d960b57")]
public partial class IsInZoneCondition : Condition
{
    [SerializeReference] public BlackboardVariable<ZoneTracker> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Zone;

    public override bool IsTrue()
    {
        return Agent.Value.IsIn(Zone.Value);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
