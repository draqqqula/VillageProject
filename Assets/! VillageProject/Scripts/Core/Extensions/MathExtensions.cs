using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
