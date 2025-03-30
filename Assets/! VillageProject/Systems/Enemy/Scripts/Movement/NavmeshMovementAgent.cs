using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshMovementAgent : MovementAgentBase<NavMeshPath>
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    private float _distance;

    public override float GetProgress()
    {
        return _navMeshAgent.remainingDistance / _distance;
    }

    public override void HandleCancellation()
    {
    }

    protected override bool TryTakePath(NavMeshPath path)
    {
        if (_navMeshAgent.SetPath(path))
        {
            _distance = _navMeshAgent.remainingDistance;
            return true;
        }
        return false;
    }

    private void OnEnable()
    {
        _navMeshAgent.enabled = true;
    }

    private void OnDisable()
    {
        _navMeshAgent.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!_navMeshAgent.hasPath)
        {
            DestinationReached?.Invoke();
        }
    }
}