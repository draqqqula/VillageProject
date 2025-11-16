using System.Collections;
using UnityEngine;

public class FloatingCrosshairPositionSignal
{
    public FloatingCrosshairPositionSignal(Vector3 eulerAngles)
    {
        EulerAngles = eulerAngles;
    }

    public Vector3 EulerAngles { get; private set; }
}