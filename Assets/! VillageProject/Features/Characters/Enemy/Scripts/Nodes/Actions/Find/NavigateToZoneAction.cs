using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Search Zone entrance", story: "[Agent] searches for [Zone] closest entrance and set [Destination]", category: "Action/Find", id: "f9a7e2069e98a5ccc013aa2638b6380a")]
public partial class NavigateToZoneAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Zone;
    [SerializeReference] public BlackboardVariable<Vector3> Destination;
    [SerializeReference] public BlackboardVariable<float> RaycastHeight = new BlackboardVariable<float>(5.0f);

    protected override Status OnStart()
    {
        try
        {
            var entrances = Zone.Value.GetComponentsInChildren<Transform>();
            var nearest = NavmeshExtensions.FirstWithShortestPath(entrances, GetPath);
            if (nearest == null)
            {
                return Status.Failure;
            }
            Destination.Value = nearest.position;
            return Status.Success;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return Status.Failure;
        }
    }

    private NavMeshPath GetPath(Transform target)
    {
        var path = new NavMeshPath();
        Agent.Value.TryBuildPathToProjection(target.position, path, RaycastHeight);
        return path;
    }
}

