using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    // Thanks to https://hollasch.github.io/ray4/Four-Space_Visualization_of_4D_Objects.html#chapter2
    public static Vector4 Cross4(Vector4 a, Vector4 b, Vector4 c)
    {
        float A, B, C, D, E, F; // Intermediate Values

        // Calculate intermediate values
        A = (b.x * c.y) - (b.y * c.x);
        B = (b.x * c.z) - (b.z * c.x);
        C = (b.x * c.w) - (b.w * c.x);
        D = (b.y * c.z) - (b.z * c.y);
        E = (b.y * c.w) - (b.w * c.y);
        F = (b.z * c.w) - (b.w * c.z);

        // Calculate the result-vector components
        Vector4 result = new Vector4(
            (a.y * F) - (a.z * E) + (a.w * D),
            -(a.x * F) + (a.z * C) - (a.w * B),
            (a.x * E) - (a.y * C) + (a.w * A),
            -(a.x * D) + (a.y * B) - (a.z * A)
            );

        return result;
    }

    // Thanks to https://hollasch.github.io/ray4/Four-Space_Visualization_of_4D_Objects.html#chapter4
    public static void GetPerspectiveViewingTransformMatrix(Vector4 from, Vector4 to, Vector4 up, Vector4 over, 
        out Vector4 wa, out Vector4 wb, out Vector4 wc, out Vector4 wd)
    {
        float norm;

        // Get the normalized wd column-vector
        wd = to - from;
        norm = wd.magnitude;
        if (norm == 0)
            throw new InvalidOperationException("To point and From point are the same.");
        wd *= 1f / norm;

        // Calculate the normalized wa column-vector
        wa = Cross4(up, over, wd);
        norm = wa.magnitude;
        if (norm == 0)
            throw new InvalidOperationException("Invalid Up vector.");
        wa *= 1f / norm;

        // Calculate the normalized wb column-vector
        wb = Cross4(over, wd, wa);
        norm = wb.magnitude;
        if (norm == 0)
            throw new InvalidOperationException("Invalid Over vector.");
        wb *= 1f / norm;

        // Calculate the wc column-vector
        wc = Cross4(wd, wa, wb);
    }

    public static Matrix4x4 CreatePerspectiveViewingTransform(Vector4 from, Vector4 to, Vector4 up, Vector4 over)
    {
        Vector4 wa, wb, wc, wd;
        GetPerspectiveViewingTransformMatrix(from, to, up, over, out wa, out wb, out wc, out wd);

        Matrix4x4 matrix = new Matrix4x4();

        // Assign the column vectors to the matrix
        matrix[0, 0] = wa.x;
        matrix[1, 0] = wa.y;
        matrix[2, 0] = wa.z;
        matrix[3, 0] = wa.w;

        matrix[0, 1] = wb.x;
        matrix[1, 1] = wb.y;
        matrix[2, 1] = wb.z;
        matrix[3, 1] = wb.w;

        matrix[0, 2] = wc.x;
        matrix[1, 2] = wc.y;
        matrix[2, 2] = wc.z;
        matrix[3, 2] = wc.w;

        matrix[0, 3] = wd.x;
        matrix[1, 3] = wd.y;
        matrix[2, 3] = wd.z;
        matrix[3, 3] = wd.w;

        return matrix;
    }

    public static Vector3 GetTransformedCoordinate(Matrix4x4 matrix, Vector4 camera, Vector4 position, float angle)
    {
        Vector4 transformed = matrix * (position - camera);
        Vector3 withoutW = new Vector3(transformed.x, transformed.y, transformed.z);
        float distance = transformed.w * Mathf.Tan(angle / 2f);
        return withoutW / (transformed.w * distance);
    }
}
