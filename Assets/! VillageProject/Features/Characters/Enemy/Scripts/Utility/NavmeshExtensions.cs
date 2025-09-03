using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public static class NavmeshExtensions
{
    public static float GetLength(this NavMeshPath path)
    {
        var result = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            result += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return result;
    }

    public static NavMeshPath GetShortest(this IEnumerable<NavMeshPath> paths)
    {
        var shortest = float.MaxValue;
        NavMeshPath result = null;
        foreach (var path in paths)
        {
            var length = path.GetLength();
            if (length < shortest)
            {
                shortest = length;
                result = path;
            }
        }
        return result;
    }

    public static T FirstWithShortestPath<T>(IEnumerable<T> targets, Func<T, NavMeshPath> selector)
    {
        var shortest = float.MaxValue;
        T result = default;
        foreach (var target in targets)
        {
            var path = selector?.Invoke(target);
            if (path.status == NavMeshPathStatus.PathInvalid)
            {
                continue;
            }
            var length = path.GetLength();
            if (length < shortest)
            {
                shortest = length;
                result = target;
            }
        }
        return result;
    }

    public static bool TryBuildPathToProjection(this NavMeshAgent agent, Vector3 position, NavMeshPath path, float raycastHeight)
    {
        return TrySurfaceProjection(position, out var destination, raycastHeight, agent.areaMask)
            && TrySurfaceProjection(agent.transform.position, out var source, raycastHeight, agent.areaMask)
            && NavMesh.CalculatePath(source, destination, agent.areaMask, path);
    }

    private static bool TrySurfaceProjection(Vector3 position, out Vector3 navMeshPoint, float raycastHeight, int areaMask)
    {
        if (NavMesh.SamplePosition(position, out var hit, raycastHeight, areaMask))
        {
            navMeshPoint = hit.position;
            return true;
        }
        navMeshPoint = Vector3.zero;
        return false;
    }
}
