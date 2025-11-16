using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AttackBlendingController : IInitializable, ITickable
{
    private const string LeftBlendParameter = "LeftBlend";
    private const string RightBlendParameter = "RightBlend";
    private const string ThrustBlendParameter = "ThrustBlend";
    private const string SeriesParameter = "Series";

    [Inject] private Animator _animator;
    [Inject] private SwingConfiguration _swingConfiguration;
    private Vector2[] _positions;
    private Vector2 _defaultPoint;
    private Vector2 _blendingPoint;

    public ReactiveProperty<AttackDirection> Direction { get; private set; } = new ReactiveProperty<AttackDirection>();
    
    public void Initialize()
    {
        _positions = new Vector2[3]
        {
            new Vector2 (0, 0),
            new Vector2 (0, 1),
            new Vector2 (0.5f, Mathf.Sqrt(3)/2)
        };
        _defaultPoint = new Vector2(0.5f, Mathf.Sqrt(3) / 6);
        Direction.Subscribe(SetSeries);
    }

    public void ForceSnap()
    {
        _blendingPoint = GetTargetPosition();
    }

    private void UpdateParameters()
    {
        var colors = MathExtensions.GetTriangleColor(_positions, _blendingPoint);
        SetBlendParameter(AttackDirection.LeftSwing, colors.x);
        SetBlendParameter(AttackDirection.RightSwing, colors.y);
        SetBlendParameter(AttackDirection.Thrust, colors.z);
    }

    private void SetSeries(AttackDirection direction)
    {
        if (direction == AttackDirection.None)
        {
            return;
        }
        _animator.SetInteger(SeriesParameter, GetSeriesIndex(direction));
    }

    private void SetBlendParameter(AttackDirection direction, float blend)
    {
        if (direction == AttackDirection.None)
        {
            return;
        }
        var key = GetParameterKey(direction);
        _animator.SetFloat(key, blend);
    }

    private string GetParameterKey(AttackDirection direction)
    {
        switch (direction)
        {
            case AttackDirection.LeftSwing: return LeftBlendParameter;
            case AttackDirection.RightSwing: return RightBlendParameter;
            case AttackDirection.Thrust: return ThrustBlendParameter;
            default: return null;
        }
    }

    private int GetSeriesIndex(AttackDirection direction)
    {
        return (int)direction;
    }

    private Vector2 GetTargetPosition()
    {
        if (Direction.CurrentValue == AttackDirection.None)
        {
            return _defaultPoint;
        }
        return _positions[(int)Direction.CurrentValue];
    }

    public void Tick()
    {
        if (Direction.CurrentValue == AttackDirection.None)
        {
            return;
        }
        _blendingPoint = Vector2.MoveTowards(
            _blendingPoint,
            GetTargetPosition(), 
            Time.deltaTime * _swingConfiguration.BlendingSpeed);
        UpdateParameters();
    }
}