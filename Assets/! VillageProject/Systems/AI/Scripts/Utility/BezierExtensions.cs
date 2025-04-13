using BezierSolution;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class BezierExtensions
{
    public static float GetProgress(this BezierSpline spline, Vector3 pointOnPath, float accuracy)
    {
        float minDistance = float.MaxValue;
        float closestProgress = 0;
        for (float t = 0; t < 1; t += accuracy)
        {
            var distance = Vector3.Distance(spline.GetPoint(t), pointOnPath);
            if (distance <= minDistance)
            {
                closestProgress = t;
                minDistance = distance;
            }
        }
        return closestProgress;
    }
}