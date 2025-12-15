using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Dashing", story: "Is [Self] dashing flag", category: "Conditions", id: "e449cc9816ae999f2dc9c6359ee3f161")]
public partial class IsDashingCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value.TryGetComponent<Dash>(out var dash))
        {
            return dash.IsCharging;
        }
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
