using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public static class MathExtensions
{
    public static Vector3 ToVector3XZ(this Vector2 vector2)
    {
        return new Vector3(vector2.x, 0, vector2.y);
    }
    public static Vector2 ToXZ(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static Vector2 AxisAnglesBetweenEuler(this Vector3 eulerA, Vector3 eulerB)
    {
        float deltaX = Mathf.DeltaAngle(eulerA.x, eulerB.x);
        float deltaY = Mathf.DeltaAngle(eulerA.y, eulerB.y);

        return new Vector2(deltaX, deltaY);
    }

    public static Vector3 MoveEulerProjected(Vector3 eulerA, Vector3 eulerB, float delta)
    {
        float dx = Mathf.DeltaAngle(eulerA.x, eulerB.x);
        float dy = Mathf.DeltaAngle(eulerA.y, eulerB.y);

        float dist = Mathf.Sqrt(dx * dx + dy * dy);

        if (dist < 0.0001f)
            return eulerB;

        if (delta >= dist)
            return new Vector3(eulerB.x, eulerB.y, eulerA.z);

        float nx = dx / dist;
        float ny = dy / dist;

        float newX = eulerA.x + nx * delta;
        float newY = eulerA.y + ny * delta;

        newX = Mathf.Repeat(newX, 360f);
        newY = Mathf.Repeat(newY, 360f);

        return new Vector3(newX, newY, eulerA.z);
    }

    public static Vector3 LerpEulerProjected(Vector3 eulerA, Vector3 eulerB, float t)
    {
        t = Mathf.Clamp01(t);

        float dx = Mathf.DeltaAngle(eulerA.x, eulerB.x);
        float dy = Mathf.DeltaAngle(eulerA.y, eulerB.y);

        float totalDist = Mathf.Sqrt(dx * dx + dy * dy);

        float delta = totalDist * t;

        return MoveEulerProjected(eulerA, eulerB, delta);
    }

    public static Vector3 SmoothDampEulerProjected(
    Vector3 current,
    Vector3 target,
    ref Vector3 velocity,
    float smoothTime)
    {
        float dx = Mathf.DeltaAngle(current.x, target.x);
        float dy = Mathf.DeltaAngle(current.y, target.y);

        Vector2 dir = new Vector2(dx, dy);
        float dist = dir.magnitude;

        if (dist < 0.0001f)
            return target;

        dir /= dist;

        float distVel = 0;
        float newDist = Mathf.SmoothDamp(dist, 0, ref distVel, smoothTime);

        float deltaDist = dist - newDist;

        return MoveEulerProjected(current, target, deltaDist);
    }

    public static Vector3 GetTriangleColor(Vector2[] edges, Vector2 point)
    {
        Vector2 v0 = edges[0];
        Vector2 v1 = edges[1];
        Vector2 v2 = edges[2];

        // Векторные направления
        Vector2 v0v1 = v1 - v0;
        Vector2 v0v2 = v2 - v0;
        Vector2 v0p = point - v0;

        // Площадные коэффициенты (через двойные площади)
        float d00 = Vector2.Dot(v0v1, v0v1);
        float d01 = Vector2.Dot(v0v1, v0v2);
        float d11 = Vector2.Dot(v0v2, v0v2);
        float d20 = Vector2.Dot(v0p, v0v1);
        float d21 = Vector2.Dot(v0p, v0v2);

        float denom = d00 * d11 - d01 * d01;
        if (Mathf.Abs(denom) < 1e-6f)
            return Vector3.zero; // Вырожденный треугольник

        // barycentric: g = v, b = w, r = u
        float g = (d11 * d20 - d01 * d21) / denom;
        float b = (d00 * d21 - d01 * d20) / denom;
        float r = 1f - g - b;

        return new Vector3(r, g, b);
    }

    public static Vector2 GetTrianglePositionFromColor(Vector2[] edges, Vector3 color)
    {
        Vector2 a = edges[0];
        Vector2 b = edges[1];
        Vector2 c = edges[2];

        float r = color.x;
        float g = color.y;
        float bW = color.z;

        float sum = r + g + bW;
        if (sum != 1f && sum > 1e-6f)
        {
            // Нормализация (на случай погрешностей)
            r /= sum;
            g /= sum;
            bW /= sum;
        }

        // Линейная комбинация вершин по барицентрическим весам
        return a * r + b * g + c * bW;
    }

    public static string ToRoman(int number)
    {
        if (number < 1) return string.Empty;
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        return string.Empty;
    }


    public static Color WithAlpha(this Color color, float a)
    {
        return new Color(color.r, color.g, color.b, a);
    }

}
