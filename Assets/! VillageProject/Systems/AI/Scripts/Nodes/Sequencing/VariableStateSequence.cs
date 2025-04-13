using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Binary State", story: "Flow to True if [State] else False. Can switch during execution", category: "Flow", id: "1c922e2af8f440b29a1dda9642fb4a6d")]
public class VariableStateSequence : BinaryStateSequenceBase
{
    [SerializeReference] public BlackboardVariable<bool> State;

    protected override bool GetValue()
    {
        return State.Value;
    }

    protected override void Subscribe(System.Action handler)
    {
        State.OnValueChanged += handler.Invoke;
    }

    protected override void Unsubscribe(System.Action handler)
    {
        State.OnValueChanged -= handler.Invoke;
    }
}
