using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Zone Entrance as target", story: "[Agent] searches for [Zone] closest entrance and set [Target]", category: "Action/Find", id: "5680cda78facabbe710649b38bec8db4")]
public partial class TargetZoneAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Zone;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> RaycastHeight = new BlackboardVariable<float>(5.0f);

    protected override Status OnStart()
    {
        var entrances = Zone.Value.GetComponentsInChildren<Transform>();
        var nearest = NavmeshExtensions.FirstWithShortestPath(entrances, GetPath);
        if (nearest == null)
        {
            return Status.Failure;
        }
        Target.Value = nearest.gameObject;
        return Status.Success;
    }

    private NavMeshPath GetPath(Transform target)
    {
        var path = new NavMeshPath();
        Agent.Value.TryBuildPathToProjection(target.position, path, RaycastHeight);
        return path;
    }
}
