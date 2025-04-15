using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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
    public static void GetViewingTransformMatrix(Vector4 from, Vector4 to, Vector4 up, Vector4 over, 
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

    // Thanks to https://hollasch.github.io/ray4/Four-Space_Visualization_of_4D_Objects.html#chapter4
    public static Vector3?[] ProjectVerticesTo3d(Vector4 wa, Vector4 wb, Vector4 wc, Vector4 wd, Vector4 camera, Vector4[] vertices, float angle)
    {
        float t = 1f / Mathf.Tan(angle / 2f);
        Vector3?[] results = new Vector3?[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector4 v = vertices[i] - camera;

            float s = t / Vector4.Dot(v, wd);

            Vector3 transformed = new Vector3(Vector4.Dot(v, wa), Vector4.Dot(v, wb), Vector4.Dot(v, wc)) * s;

            results[i] = IsVector3NaNOrInfinity(transformed) ? null : transformed;
        }

        return results;
    }

    public static float Mod(float x, float m)
    {
        return (x % m + m) % m;
    }

    public static bool IsVector3NaNOrInfinity(Vector3 vector)
            => float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z) ||
               float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z);
    private static Vector4 FindIntersectionOnPlane(Vector4 A, Vector4 B, float wPlane)
    {
        float denominator = B.w - A.w;
        if (Math.Abs(denominator) < 1e-6)
        {
            // Handle the case where the line is parallel to the plane
            return A; // or some other logic
        }
        float t = (wPlane - A.w) / denominator;
        return A + t * (B - A);
    }

    public static void ApplyIntersectioning(ref Vector4[] vertices, ref int[][] connections)
    {
        Vector4[] originalVertices = vertices.ToArray();
        int[][] originalConnections = connections.ToArray();

        List<Vector4> updatedVertices = new();
        List<int[]> updatedConnections = new();
        HashSet<(int, int)> addedConnections = new();

        for (int i = 0; i < originalVertices.Length; i++)
        {
            if (originalVertices[i].w <= 0)
            {
                IEnumerable<int[]> connectionPairs = originalConnections
                    .Where(arr => (arr[0] == i || arr[1] == i) &&
                                  (originalVertices[arr[0]].w > 0 || originalVertices[arr[1]].w > 0));

                IEnumerable<Vector4> vectorsConnectedToThis = connectionPairs
                    .Select(arr => originalVertices[arr[0] == i ? arr[1] : arr[0]])
                    .ToList();

                IEnumerable<Vector4> intersectedVectors = vectorsConnectedToThis
                    .Select(v => FindIntersectionOnPlane(originalVertices[i], v, 0.1f));

                updatedVertices.AddRange(intersectedVectors);

                foreach (int[] pair in connectionPairs)
                {
                    int a = pair[0], b = pair[1];
                    if (!addedConnections.Contains((a, b)) && !addedConnections.Contains((b, a)))
                    {
                        updatedConnections.Add(pair);
                        addedConnections.Add((a, b));
                    }
                }
                continue;
            }

            updatedVertices.Add(originalVertices[i]);
            foreach (int[] connection in originalConnections.Where(c => c[0] == i || c[1] == i))
            {
                int a = connection[0], b = connection[1];
                if (!addedConnections.Contains((a, b)) && !addedConnections.Contains((b, a)))
                {
                    updatedConnections.Add(connection);
                    addedConnections.Add((a, b));
                }
            }
        }

        vertices = updatedVertices.ToArray();
        connections = updatedConnections.ToArray();
    }
}
