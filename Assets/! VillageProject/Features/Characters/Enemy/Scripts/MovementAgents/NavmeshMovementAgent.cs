using R3;
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
    [SerializeField] private Speed _speed;
    [SerializeField] private float _speedModifier;
    private IDestination _destination;
    private Vector3 _cachedDestination;
    private float _distance;

    public bool CanStop {private get; set;}
    public bool CanChangeDestination {private get; set;}
    
    public override float GetProgress()
    {
        return _navMeshAgent.remainingDistance / _distance;
    }

    public override void HandleCancellation()
    {
        base.HandleCancellation();
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

    private void Awake()
    {
        _speed.Value
            .Subscribe(HandleSpeedChanged)
            .AddTo(this);
        
        CanChangeDestination = true;
    }

    private void OnEnable()
    {
        ConnectToNavmesh();
    }

    private void HandleSpeedChanged(float newValue)
    {
        _navMeshAgent.speed = newValue * _speedModifier;
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
                _navMeshAgent.isStopped = false;
            }
        }

        if (!_navMeshAgent.hasPath)
        {
            SignalWorkCompleted();
        }
    }

    public override float GetVelocityPerSecond()
    {
        return _navMeshAgent.velocity.magnitude;
    }

    public void StopAgent()
    {
        if (!CanStop) return;
        
        _cachedDestination = Vector3.zero;
        _destination = null;
        
        _navMeshAgent.isStopped = true;
        HandleWorkCompleted();
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
        if (!CanChangeDestination)
        {
            source = null;
            return false;
        }

        _destination = new Vector3Destination(instructions);
        return TryBuildPathToDestination(out source);
    }

    public bool TrySetInstructions(GameObject instructions, out IWorkEventSource<WorkResult> source)
    {
        if (!CanChangeDestination)
        {
            source = null;
            return false;
        }
        
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