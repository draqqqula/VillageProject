using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TransformDestination : IDestination
{
    private Transform _transform;

    public TransformDestination(Transform transform)
    { 
        _transform = transform; 
    }

    public Vector3 GetPosition()
    {
        return _transform.position;
    }
}
