using System;
using UnityEngine;

public class ScaleWithDistance : MonoBehaviour
{
    [SerializeField] private AnimationCurve _scaleOverDistance;

    private void Update()
    {
        var t = Vector3.Distance(Camera.main.transform.position, transform.position);
        transform.localScale = Vector3.one * _scaleOverDistance.Evaluate(t);
    }
}

