using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is cooldown dashing", story: "Is [Self] dash on cooldown", category: "Conditions", id: "8fe3cf3828b2860aef5127da754d7dea")]
public partial class IsCooldownDashingCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value.TryGetComponent<Dash>(out var dash))
        {
            return dash.IsCooldown;
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
