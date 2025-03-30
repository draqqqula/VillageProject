using BezierSolution;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class SurfaceNavigation : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private LayerMask _raycastLayerMask;
    [SerializeField] private float _raycastDistance;

    [Header("Navmesh")]
    [SerializeField] private float _navMeshSearchDistance;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private MovementAgentBase<NavMeshPath> _agent;

    public void GoTo(Vector3 position)
    {
        var path = new NavMeshPath();
        TryBuildPathTo(position, path);
        _agent.TryTakePathWithCallback(path);
    }

    public bool TryBuildPathTo(Vector3 position, NavMeshPath path)
    {
        return TryGetNavmeshPoint(transform.position, out var source)
            && TryGetNavmeshPoint(position, out var destination)
            && _navMeshAgent.CalculatePath(destination, path);
    }

    private bool TryGetNavmeshPoint(Vector3 position, out Vector3 navMeshPoint)
    {
        if (Physics.Raycast(position, Vector3.down, out var physicsHit, _raycastDistance, _raycastLayerMask) &&
            NavMesh.SamplePosition(physicsHit.point, out var navMeshHit, _navMeshSearchDistance, _navMeshAgent.areaMask))
        {
            navMeshPoint = physicsHit.point;
            return true;
        }
        navMeshPoint = Vector3.zero;
        return false;
    }
}
