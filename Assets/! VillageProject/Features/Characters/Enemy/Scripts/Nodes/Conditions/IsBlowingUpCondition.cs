using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is blowing up", story: "Is [Self] blowing up", category: "Conditions", id: "53c29ec440f58b6747067cbfc6b12d6a")]
public partial class IsBlowingUpCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    public override bool IsTrue()
    {
        if (Self.Value.TryGetComponent<BlowUp>(out var blowUp))
        {
            Debug.Log(blowUp.IsBlowing);
            return blowUp.IsBlowing;
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
