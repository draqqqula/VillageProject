using BezierSolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BezierCurveMovementAgent : MovementAgentBase<BezierPathWithStart>
{
    private float _progressDistance;

    [SerializeField] private float _speed;
    [SerializeField] private float _elevation;
    [SerializeField] private BezierSpline _spline;

    [Header("Raycast")]
    [SerializeField] private LayerMask _raycastLayerMask;
    [SerializeField] private float _raycastDistance;

    public override float GetProgress()
    {
        return _progressDistance / _spline.length;
    }

    public override void HandleCancellation()
    {
    }

    protected override bool TryTakePath(BezierPathWithStart path)
    {
        _spline = path.Spline;
        _progressDistance = _spline.length * path.InitialProgress;

        return true;
    }

    private void FixedUpdate()
    {
        _progressDistance = Mathf.Clamp(_progressDistance + _speed, 0, _spline.length);
        var pointInSpline = _spline.GetPoint(GetProgress());
        if (Physics.Raycast(pointInSpline, Vector3.down, out var hit, _raycastDistance, _raycastLayerMask))
        {
            transform.position = hit.point + Vector3.up * _elevation;
        }
        if (_progressDistance == _spline.length)
        {
            DestinationReached?.Invoke();
        }
    }
}
