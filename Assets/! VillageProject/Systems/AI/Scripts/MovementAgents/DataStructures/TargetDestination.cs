using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TargetDestination : IDestination
{
    private Target _target;
    private Transform _searcher;
    public TargetDestination(Target target, Transform searcher)
    {
        _searcher = searcher;
        _target = target;
    }

    public Vector3 GetPosition()
    {
        var targetPosition = _target.transform.position;
        var direction = (_searcher.position - targetPosition)
            .ToXZ()
            .ToVector3XZ().normalized;
        return _target.transform.position + direction;
    }
}
