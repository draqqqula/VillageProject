using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Vector3Destination : IDestination
{
    private Vector3 _position;

    public Vector3Destination(Vector3 position)
    {
        _position = position;
    }

    public Vector3 GetPosition()
    {
        return _position;
    }
}
