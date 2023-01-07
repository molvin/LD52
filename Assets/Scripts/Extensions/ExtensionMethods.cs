using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static float Dist2D(this Vector3 Self, Vector3 Other)
    {
        Vector2 a = new Vector2(Self.x, Self.z);
        Vector2 b = new Vector2(Other.x, Other.z);
        return Vector2.Distance(a, b);
    }
}