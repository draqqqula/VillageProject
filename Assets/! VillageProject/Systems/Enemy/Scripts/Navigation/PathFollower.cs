using BezierSolution;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private BezierSpline _path;
    [SerializeField] private SurfaceNavigation _freeAgent;
    [SerializeField] private MovementAgentBase<BezierPathWithStart> _pathAgent;
    [SerializeField] private MovementOrchestrator _orchestrator;
    [SerializeField] private float _minDelta;
    [SerializeField] private float _searchStep;

    private void Reset()
    {
        _freeAgent = GetComponent<SurfaceNavigation>();
        _pathAgent = GetComponent<MovementAgentBase<BezierPathWithStart>>();
        _orchestrator = GetComponent<MovementOrchestrator>();
    }

    public void ReturnToPath()
    {
        var nearest = _path.FindNearestPointTo(transform.position);
        _freeAgent.GoTo(nearest);
        _orchestrator.OnNeutralEnter.AddListener(StickToPath);
    }

    private void StickToPath()
    {
        _orchestrator.OnNeutralEnter.RemoveListener(StickToPath);
        var nearest = _path.FindNearestPointTo(transform.position);
        _pathAgent.TryTakePathWithCallback(new BezierPathWithStart(_path, GetProgress(nearest)));
    }

    private float GetProgress(Vector3 pointOnPath)
    {
        float minDistance = float.MaxValue;
        float closest = 0;
        for (float t = 0; t < 1; t += _searchStep)
        {
            var distance = Vector3.Distance(_path.GetPoint(t), pointOnPath);
            if (distance <= minDistance)
            {
                closest = t;
                minDistance = distance;
            }
        }
        return closest;
    }
}