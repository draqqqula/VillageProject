using UnityEngine;

public interface IProbeBlendProvider
{
    public Vector3 ProbeA { get; }

    public Vector3 ProbeB { get; }

    public float Blend { get; }
}