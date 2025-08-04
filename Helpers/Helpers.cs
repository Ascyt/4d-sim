using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers
{
    /// <summary>
    /// Calculates the cross product of three 4D vectors.<br />
    /// Thanks to https://hollasch.github.io/ray4/Four-Space_Visualization_of_4D_Objects.html
    /// </summary>
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

    /// <summary>
    /// Calculates the viewing transform matrix for a 4D camera.<br />
    /// Thanks to https://hollasch.github.io/ray4/Four-Space_Visualization_of_4D_Objects.html
    /// </summary>
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

    /// <summary>
    /// Projects 4D vertices to 3D space using the viewing transform matrix defined by wa, wb, wc, wd and camera position.<br />
    /// Thanks to https://hollasch.github.io/ray4/Four-Space_Visualization_of_4D_Objects.html
    /// </summary>
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

    /// <summary>
    /// Finds the point on the line between A and B where w=wPlane. <br />
    /// Assumes that A.w and B.w are on opposite sides of wPlane.
    /// </summary>
    private static Vector4 FindIntersectionOnPlane(Vector4 A, Vector4 B, float wPlane)
    {
        // Calculate how far along the segment the plane occurs
        float t = (wPlane - A.w) / (B.w - A.w);
        return A + t * (B - A);
    }

    /// <summary>
    /// Clip connections (edges) so that any connections that cross from behind to in front of the camera are replaced by an intersection on a w-plane.<br />
    /// The camera is at w==0 (with vertices in front having positive w); vertices behind (w&lt;=0) are clipped.<br /><br />
    /// 
    /// Note that this was made using an LLM.
    /// </summary>
    /// <param name="vertices">Array of vertices relative to the camera</param>
    /// <param name="connections">Each connection is an int[2] pair of indices into the vertices array</param>
    public static void ApplyIntersectioning(ref Vector4[] vertices, ref int[][] connections)
    {
        // The new vertices and connections that will be produced.
        List<Vector4> newVertices = new List<Vector4>();
        List<int[]> newConnections = new List<int[]>();

        // Map from original vertex index (with vertex in front) to new vertex index (so we add each only once).
        Dictionary<int, int> frontVertexMap = new Dictionary<int, int>();

        // For each edge that requires clipping, we store the computed intersection vertex 
        // keyed by the pair {behindIndex, frontIndex} (ordered) so that we don’t compute intersections twice.
        Dictionary<Tuple<int, int>, int> intersectionVertexMap = new Dictionary<Tuple<int, int>, int>();

        const float NEAR_PLANE = 1f / 16f;

        // Process each connection (edge) exactly once.
        for (int edgeIndex = 0; edgeIndex < connections.Length; edgeIndex++)
        {
            int[] connection = connections[edgeIndex];
            if (connection.Length != 2)
            {
                continue; // ignore badly formed connections.
            }

            int indexA = connection[0];
            int indexB = connection[1];
            Vector4 pointA = vertices[indexA];
            Vector4 pointB = vertices[indexB];

            bool aInFront = (pointA.w > 0);
            bool bInFront = (pointB.w > 0);

            // Case 1: Both endpoints are in front of the camera.
            if (aInFront && bInFront)
            {
                // Add point A if not already added.
                if (!frontVertexMap.ContainsKey(indexA))
                {
                    frontVertexMap[indexA] = newVertices.Count;
                    newVertices.Add(pointA);
                }
                // Likewise, add point B.
                if (!frontVertexMap.ContainsKey(indexB))
                {
                    frontVertexMap[indexB] = newVertices.Count;
                    newVertices.Add(pointB);
                }
                // Create an edge using the new indices.
                newConnections.Add(new int[] { frontVertexMap[indexA], frontVertexMap[indexB] });
            }
            // Case 2: One vertex is in front and the other is behind.
            // (Edges from behind to in front are “clipped”.)
            else if (aInFront ^ bInFront) // exclusive or: exactly one must be true.
            {
                int frontIndex;    // original index of the vertex that is in front
                int behindIndex;   // original index of the vertex behind (or on the camera)
                Vector4 frontVertex;   // actual position (in front)
                Vector4 behindVertex;  // actual position (behind)

                if (aInFront)
                {
                    frontIndex = indexA;
                    behindIndex = indexB;
                    frontVertex = pointA;
                    behindVertex = pointB;
                }
                else
                {
                    frontIndex = indexB;
                    behindIndex = indexA;
                    frontVertex = pointB;
                    behindVertex = pointA;
                }
                // Compute the intersection point along the edge.
                Vector4 intersection = FindIntersectionOnPlane(behindVertex, frontVertex, NEAR_PLANE);

                // Add the front vertex to the new vertices (if not already added).
                if (!frontVertexMap.ContainsKey(frontIndex))
                {
                    frontVertexMap[frontIndex] = newVertices.Count;
                    newVertices.Add(frontVertex);
                }
                // Create an ordered key to look-up/store the intersection vertex.
                Tuple<int, int> edgeKey = (behindIndex < frontIndex) ?
                                          new Tuple<int, int>(behindIndex, frontIndex) :
                                          new Tuple<int, int>(frontIndex, behindIndex);
                int newIntersectionIndex;
                if (!intersectionVertexMap.TryGetValue(edgeKey, out newIntersectionIndex))
                {
                    newIntersectionIndex = newVertices.Count;
                    newVertices.Add(intersection);
                    intersectionVertexMap[edgeKey] = newIntersectionIndex;
                }

                // Build an edge from the intersection point to the front vertex.
                newConnections.Add(new int[] { newIntersectionIndex, frontVertexMap[frontIndex] });
            }
            // Case 3: Both vertices are behind (or exactly on) the camera.
            // In this case we do not render the edge.
            // (You might add more handling if desired, e.g. for edges on the plane.)
        }

        // Replace original arrays with our new clipped versions.
        vertices = newVertices.ToArray();
        connections = newConnections.ToArray();
    }
    public static int ToInt<T>(this T value) where T : Enum
    {
        return Convert.ToInt32(value);
    }
    public static T RunOnEnumAsInt<T>(this T value, Func<int, int> func) where T : Enum
    {
        int numericValue = Convert.ToInt32(value);
        int newNumericValue = func(numericValue);
        return (T)Enum.ToObject(typeof(T), newNumericValue);
    }
}
