using R3;
using System;
using UnityEngine;

public class FloatingCrosshair : MonoBehaviour
{
    private Camera _targetCamera;

    [NonSerialized] public Vector2 EulerAngles = Vector2.zero;


    private void Awake()
    {
        _targetCamera = Camera.main;
    }

    private void Update()
    {
        var normalizedWorldPosition = ToNormalizedWorldPosition(EulerAngles);
        var worldPositionWithOffset = normalizedWorldPosition + _targetCamera.transform.position;
        transform.position = _targetCamera.WorldToScreenPoint(worldPositionWithOffset);
    }

    private Vector3 ToNormalizedWorldPosition(Vector2 eulerAngles)
    {
        Quaternion rot = Quaternion.Euler(eulerAngles.x, eulerAngles.y, 0f);
        return rot * Vector3.forward;
    }
}
