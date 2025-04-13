using BezierSolution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BezierCurveMovementAgent : MovementWorkerBase<BezierPathWithStart>, IMovementInsructionsAccept<BezierSpline>
{
    private float _progressDistance;

    [SerializeField] private float _speed;
    [SerializeField] private float _elevation;
    [SerializeField] private BezierSpline _spline;

    [Header("Raycast")]
    [SerializeField] private LayerMask _raycastLayerMask;
    [SerializeField] private float _raycastDistance;
    [SerializeField] private float _accuracy = 0.01f;

    public override float GetProgress()
    {
        return _progressDistance / _spline.length;
    }

    public override float GetVelocity()
    {
        return _speed;
    }

    public override void HandleCancellation()
    {
    }

    public bool TrySetInstructions(BezierSpline instructions, out IWorkEventSource<WorkResult> source)
    {
        return TrySetInstructions(
            new BezierPathWithStart(instructions, instructions.GetProgress(transform.position, _accuracy)), 
            out source);
    }

    protected override bool TryAcceptInstructions(BezierPathWithStart path)
    {
        _spline = path.Spline;
        _progressDistance = _spline.length * path.InitialProgress;

        return true;
    }

    private void FixedUpdate()
    {
        _progressDistance = Mathf.Clamp(_progressDistance + _speed, 0, _spline.length);
        var progress = GetProgress();
        var pointInSpline = _spline.GetPoint(progress);
        var direction = _spline.GetNormal(progress);
        if (Physics.Raycast(pointInSpline, Vector3.down, out var hit, _raycastDistance, _raycastLayerMask))
        {
            transform.position = hit.point + Vector3.up * _elevation;
            transform.rotation = Quaternion.Euler(direction);
        }
        if (_progressDistance == _spline.length)
        {
            SignalWorkCompleted();
        }
    }
}
