using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static float Dist2D(this Vector3 Self, Vector3 Other)
    {
        Vector3 a = Self;
        a.y = 0.0f;
        Vector3 b = Other;
        b.y = 0.0f;
        return Vector3.Distance(a, b);
    }
}