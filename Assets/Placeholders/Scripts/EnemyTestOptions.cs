using BezierSolution;
using UnityEngine;

public class EnemyTestOptions : MonoBehaviour
{
    [SerializeField] private BezierSpline spline;
    [SerializeField, Range(0, 1)] private float initialProgress;
    [SerializeField] protected Transform target;

    [SerializeField] protected BezierCurveMovementAgent bezier;

    [ContextMenu("Set target")]
    private void SetTarget()
    {
    }

    [ContextMenu("Set path")]
    private void SetPath()
    {
        bezier.TrySetInstructions(new BezierPathWithStart(spline, initialProgress), out var source);
    }

    private void Reset()
    {
        bezier = GetComponent<BezierCurveMovementAgent>();
    }
}
