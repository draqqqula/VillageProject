using BezierSolution;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    public enum NavigationState
    {
        OnRoute,
        Free
    }

    private BezierSpline _path;
    private NavMeshAgent _navMeshAgent;

    public NavigationState State {  get; private set; }

    public void TakePath(BezierSpline path)
    {
        _path = path;
    }

    public void SetTarget(Transform target)
    {
    }

    private void FixedUpdate()
    {
    }
}
