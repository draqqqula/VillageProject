using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;

public class NavmeshMovementAgent : MovementWorkerBase<NavMeshPath>, 
    IMovementInsructionsAccept<Vector3>, 
    IMovementInsructionsAccept<GameObject>
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private float _raycastHeight;
    private IDestination _destination;
    private Vector3 _cachedDestination;
    private float _distance;

    public override float GetProgress()
    {
        return _navMeshAgent.remainingDistance / _distance;
    }

    public override void HandleCancellation()
    {

    }

    protected override bool TryAcceptInstructions(NavMeshPath path)
    {
        ConnectToNavmesh();
        if (_navMeshAgent.SetPath(path))
        {
            _distance = _navMeshAgent.remainingDistance;
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        ConnectToNavmesh();
    }

    private void OnDisable()
    {
        _navMeshAgent.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_destination != null
            && _destination.GetPosition() != _cachedDestination)
        {
            var path = new NavMeshPath();
            if (TryBuildPathToDestination(path)
                && TryAcceptInstructions(path))
            {
                _cachedDestination = _destination.GetPosition();
            }
        }

        if (!_navMeshAgent.hasPath)
        {
            SignalWorkCompleted();
        }
    }

    public override float GetVelocity()
    {
        return _navMeshAgent.velocity.magnitude;
    }

    private void ConnectToNavmesh()
    {
        _navMeshAgent.enabled = true;
        if (!_navMeshAgent.isOnNavMesh
            && _navMeshAgent.FindClosestEdge(out var hit))
        {
            _navMeshAgent.Warp(hit.position);
        }
    }    

    public bool TrySetInstructions(Vector3 instructions, out IWorkEventSource<WorkResult> source)
    {
        _destination = new Vector3Destination(instructions);
        return TryBuildPathToDestination(out source);
    }

    public bool TrySetInstructions(GameObject instructions, out IWorkEventSource<WorkResult> source)
    {
        _destination = new TransformDestination(instructions.transform);
        return TryBuildPathToDestination(out source);
    }

    private bool TryBuildPathToDestination(NavMeshPath path)
    {
        return NavmeshExtensions.TryBuildPathToProjection(_navMeshAgent, _destination.GetPosition(), path, _raycastHeight);
    }

    private bool TryBuildPathToDestination(out IWorkEventSource<WorkResult> source)
    {
        var path = new NavMeshPath();
        if (TryBuildPathToDestination(path))
        {
            return TrySetInstructions(path, out source);
        }
        source = null;
        return false;
    }
}