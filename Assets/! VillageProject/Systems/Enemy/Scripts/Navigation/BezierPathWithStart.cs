using BezierSolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public readonly struct BezierPathWithStart
{
    public readonly BezierSpline Spline;
    public readonly float InitialProgress;

    public BezierPathWithStart(BezierSpline spline, float initialProgress)
    {
        Spline = spline;
        InitialProgress = initialProgress;
    }
}
