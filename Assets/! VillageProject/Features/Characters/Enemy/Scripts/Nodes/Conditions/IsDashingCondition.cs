using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is dashing", story: "Is [Self] dashing", category: "Conditions", id: "2befdd1bd5495878e3b54cea4073de46")]
public partial class IsDashingCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value.TryGetComponent<Dash>(out var dash))
        {
            return dash.IsDashing;
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
