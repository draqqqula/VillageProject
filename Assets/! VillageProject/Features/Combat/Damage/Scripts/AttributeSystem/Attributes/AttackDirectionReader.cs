using System.Collections;
using UnityEngine;
using Zenject;

[GenerateDamageAttribute(inject: true)]
public class AttackDirectionReader
{
    [Inject(Id = "LeftSlash")] private IAnimationWindowListener _leftSwingListener;
    [Inject(Id = "RightSlash")] private IAnimationWindowListener _rightSwingListener;
    [Inject(Id = "ThrustAttack")] private IAnimationWindowListener _thrustListener;

    public AttackDirection GetDirection()
    {
        if (_leftSwingListener.IsActive.CurrentValue)
        {
            return AttackDirection.LeftSwing;
        }
        else if (_rightSwingListener.IsActive.CurrentValue)
        {
            return AttackDirection.RightSwing;
        }
        else if (_thrustListener.IsActive.CurrentValue)
        {
            return AttackDirection.Thrust;
        }
        return AttackDirection.None;
    }
}