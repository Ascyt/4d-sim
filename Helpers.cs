using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    // Thanks to https://hollasch.github.io/ray4/Four-Space_Visualization_of_4D_Objects.html#chapter2
    public static Vector4 Cross(this Vector4 v, Vector4 a, Vector4 b)
    {
        float A, B, C, D, E, F; // Intermediate Values

        // Calculate intermediate values
        A = (a.x * b.y) - (a.y * b.x);
        B = (a.x * b.z) - (a.z * b.x);
        C = (a.x * b.w) - (a.w * b.x);
        D = (a.y * b.z) - (a.z * b.y);
        E = (a.y * b.w) - (a.w * b.y);
        F = (a.z * b.w) - (a.w * b.z);


        // Calculate the result-vector components
        Vector4 result = new Vector4(
            (v.y * F) - (v.z * E) + (v.w * D),
            -(v.x * F) + (v.z * C) - (v.w * B),
            (v.x * E) - (v.y * C) + (v.w * A),
            -(v.x * D) + (v.y * B) - (v.z * A)
            );

        return result;
    }
}
