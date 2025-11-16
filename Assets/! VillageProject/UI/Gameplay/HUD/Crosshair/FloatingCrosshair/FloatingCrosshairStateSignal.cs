using System.Collections;
using UnityEngine;

public class FloatingCrosshairStateSignal
{
    public FloatingCrosshairStateSignal(bool enabled)
    {
        Enabled = enabled;
    }

    public bool Enabled { get; private set; }
}