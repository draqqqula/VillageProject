using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Nearest Target", story: "Find [Target] with shortest path from [Agent] with tag: [Tag] and save [Position]", category: "Action/Navigation", id: "36cd5cb8994abc1bcda1a918f9885f44")]
public partial class NavigateToClosestAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<string> Tag;
    [SerializeReference] public BlackboardVariable<Vector3> Position;
    [SerializeReference] public BlackboardVariable<float> RaycastHeight = new BlackboardVariable<float>(5.0f);

    protected override Status OnStart()
    {
        var targets = GameObject.FindGameObjectsWithTag(Tag);
        GameObject result = null;

        result = NavmeshExtensions.FirstWithShortestPath(targets, GetPath);
        Position.Value = result?.transform.position ?? Vector3.zero;

        return result == null ? Status.Failure : Status.Success;
    }

    private NavMeshPath GetPath(GameObject target)
    {
        var path = new NavMeshPath();
        Agent.Value.TryBuildPathToProjection(target.transform.position, path, RaycastHeight);
        return path;
    }
}

