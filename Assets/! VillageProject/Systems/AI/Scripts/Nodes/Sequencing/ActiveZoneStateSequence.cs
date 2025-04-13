using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using UnityEngine.Events;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Active Zone State", story: "[Tracker] is in [Zone]", category: "Flow", id: "05510bb540f8adb0e27c36169eb7bc00")]
public partial class ActiveZoneStateSequence : BinaryStateSequenceBase
{
    [SerializeReference] public BlackboardVariable<ZoneTracker> Tracker;
    [SerializeReference] public BlackboardVariable<GameObject> Zone;
    private event System.Action Callback;

    protected override bool GetValue()
    {
        return ReferenceEquals(Tracker.Value.ActiveZone, Zone.Value);
    }

    protected override void Subscribe(System.Action handler)
    {
        Callback += handler;
        Tracker.Value.OnNewActiveZone.AddListener(InvokeCallback);
    }

    protected override void Unsubscribe(System.Action handler)
    {
        Callback -= handler;
        Tracker.Value.OnNewActiveZone.RemoveListener(InvokeCallback);
    }

    private void InvokeCallback(GameObject _)
    {
        Callback?.Invoke();
    }
}

