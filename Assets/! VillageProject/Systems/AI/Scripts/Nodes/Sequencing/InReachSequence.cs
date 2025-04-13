using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "In Reach", story: "Distance between [Agent] and [Target] is less than [Reach]", category: "Flow", id: "34b002e779324f2a7ee7a440758e9914")]
public partial class InReachSequence : BinaryStateSequenceBase
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Target> Target;
    [SerializeReference] public BlackboardVariable<float> Reach;

    private event System.Action _distanceChanged;
    private float _distance;

    protected override bool GetValue()
    {
        return Vector2.Distance(
            Agent.Value.transform.position.ToXZ(), 
            Target.Value.transform.position.ToXZ()) < Reach.Value;
    }

    protected override void Subscribe(System.Action handler)
    {
        var searcher = Agent.Value.GetComponentInChildren<SearchForTarget>();
        if (searcher != null)
        {
            _distanceChanged += handler;
            searcher.OnDistanceToTargetUpdated.AddListener(HandleDistanceUpdated);
        }
    }

    protected override void Unsubscribe(System.Action handler)
    {
        var searcher = Agent.Value.GetComponentInChildren<SearchForTarget>();
        if (searcher != null)
        {
            _distanceChanged -= handler;
            searcher.OnDistanceToTargetUpdated.RemoveListener(HandleDistanceUpdated);
        }
    }

    private void HandleDistanceUpdated(Target target, float distance)
    {
        if (ReferenceEquals(target, Target.Value))
        {
            _distance = distance;
            _distanceChanged?.Invoke();
        }
    }
}

