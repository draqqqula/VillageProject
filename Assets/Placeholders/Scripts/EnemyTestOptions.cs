using BezierSolution;
using UnityEngine;

public class EnemyTestOptions : MonoBehaviour
{
    [SerializeField] private BezierSpline spline;
    [SerializeField, Range(0, 1)] private float initialProgress;
    [SerializeField] protected Transform target;

    [SerializeField] protected SurfaceNavigation nav;
    [SerializeField] protected BezierCurveMovementAgent bezier;
    [SerializeField] private PathFollower pathFollower;

    [ContextMenu("Set target")]
    private void SetTarget()
    {
        nav.GoTo(target.position);
    }

    [ContextMenu("Set path")]
    private void SetPath()
    {
        bezier.TryTakePathWithCallback(new BezierPathWithStart(spline, initialProgress));
    }

    [ContextMenu("Go to path")]
    private void GoToPath()
    {
        pathFollower.ReturnToPath();
    }

    private void Reset()
    {
        nav = GetComponent<SurfaceNavigation>();
        bezier = GetComponent<BezierCurveMovementAgent>();
    }
}
