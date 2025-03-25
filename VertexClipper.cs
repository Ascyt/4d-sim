using System.Collections.Generic;
using UnityEngine;

public static class VertexClipper
{
    // A small epsilon to account for floating point error.
    private const float EPS = 1e-5f;

    // A simple plane representation: plane equation is (normal · x + d == 0)
    // and points inside are those for which (normal · x + d <= 0).
    private struct PlaneData
    {
        public Vector3 normal;
        public float d;
        public PlaneData(Vector3 n, float dVal)
        {
            normal = n;
            d = dVal;
        }
        // Evaluates f(x)=n·x + d.
        public float Evaluate(Vector3 point)
        {
            return Vector3.Dot(normal, point) + d;
        }
    }

    // Clip the convex hull given by vertices "points" (in object–local space),
    // with the convex hull center "position" (i.e. world–position of the center),
    // by a 10×10×10 cube centered at the origin (from -5, -5, -5 to 5,5,5).
    // Return the new vertices in object–local coordinates.
    public static Vector3[] ClipVertices(Vector3[] points, Vector3 position)
    {
        // Convert local points to world space:
        Vector3[] worldPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            worldPoints[i] = points[i] + position;

        // If ALL points are inside the cube then nothing is clipped.
        bool needsClip = false;
        foreach (Vector3 pt in worldPoints)
        {
            if (pt.x < -5 - EPS || pt.x > 5 + EPS ||
                pt.y < -5 - EPS || pt.y > 5 + EPS ||
                pt.z < -5 - EPS || pt.z > 5 + EPS)
            {
                needsClip = true;
                break;
            }
        }
        if (!needsClip)
            return points; // No clipping needed.

        // Gather half-spaces for cube. Our convention: the half-space is defined
        // by n · x + d <= 0.
        List<PlaneData> clipPlanes = new List<PlaneData>();
        // For cube face at x = 5: inside is x <= 5.
        clipPlanes.Add(new PlaneData(new Vector3(1, 0, 0), -5));
        // For cube face at x = -5: inside is x >= -5  <=>  -x <= 5.
        clipPlanes.Add(new PlaneData(new Vector3(-1, 0, 0), -5));
        // y = 5
        clipPlanes.Add(new PlaneData(new Vector3(0, 1, 0), -5));
        // y = -5
        clipPlanes.Add(new PlaneData(new Vector3(0, -1, 0), -5));
        // z = 5
        clipPlanes.Add(new PlaneData(new Vector3(0, 0, 1), -5));
        // z = -5
        clipPlanes.Add(new PlaneData(new Vector3(0, 0, -1), -5));

        // Now, include the half-spaces that define the original convex hull.
        // We already know that the hull center in world space is "position".
        List<PlaneData> hullPlanes = ComputeHullFaces(worldPoints, position);
        clipPlanes.AddRange(hullPlanes);

        // Now, the clipped polyhedron is the intersection of all these half-spaces.
        // One way to compute its vertices is to take every triple of planes and
        // compute their intersection point, then keep the point if it satisfies every half-space.
        List<Vector3> outputPoints = new List<Vector3>();
        int planeCount = clipPlanes.Count;
        for (int i = 0; i < planeCount; i++)
        {
            for (int j = i + 1; j < planeCount; j++)
            {
                for (int k = j + 1; k < planeCount; k++)
                {
                    Vector3? ip = IntersectThreePlanes(clipPlanes[i], clipPlanes[j], clipPlanes[k]);
                    if (ip.HasValue)
                    {
                        Vector3 pCandidate = ip.Value;
                        bool inside = true;
                        foreach (PlaneData plane in clipPlanes)
                        {
                            if (plane.Evaluate(pCandidate) > EPS)
                            {
                                inside = false;
                                break;
                            }
                        }
                        if (inside)
                        {
                            // Avoid duplicate vertices.
                            bool duplicate = false;
                            for (int idx = 0; idx < outputPoints.Count; idx++)
                            {
                                if ((outputPoints[idx] - pCandidate).sqrMagnitude < EPS * EPS)
                                {
                                    duplicate = true;
                                    break;
                                }
                            }
                            if (!duplicate)
                                outputPoints.Add(pCandidate);
                        }
                    }
                }
            }
        }

        // Convert output points back to object-local space (subtract the object's position).
        for (int i = 0; i < outputPoints.Count; i++)
            outputPoints[i] -= position;

        return outputPoints.ToArray();
    }

    // Computes the convex hull face planes of the input polyhedron (given by its vertices in world space).
    // It tests every combination of three vertices. For each candidate face, it checks if all other vertices lie
    // on one side. The plane is then oriented so that the interior (which contains center "c") satisfies n·x + d <= 0.
    private static List<PlaneData> ComputeHullFaces(Vector3[] pts, Vector3 c)
    {
        List<PlaneData> faces = new List<PlaneData>();
        int n = pts.Length;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                for (int k = j + 1; k < n; k++)
                {
                    Vector3 p0 = pts[i], p1 = pts[j], p2 = pts[k];
                    // Compute the normal (if non-degenerate).
                    Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0);
                    if (normal.sqrMagnitude < EPS * EPS)
                        continue; // Degenerate triangle.
                    normal.Normalize();

                    // Compute d so that plane: normal·x + d = 0 passes through p0.
                    float d = -Vector3.Dot(normal, p0);

                    // Check on which side all points lie.
                    int positive = 0, negative = 0;
                    for (int m = 0; m < n; m++)
                    {
                        float dist = Vector3.Dot(normal, pts[m]) + d;
                        if (dist > EPS)
                            positive++;
                        else if (dist < -EPS)
                            negative++;
                    }
                    // One of the counts must be zero if this is a face.
                    if (positive != 0 && negative != 0)
                        continue;

                    // Orient the plane so that the center c is inside.
                    if (Vector3.Dot(normal, c) + d > 0)
                    {
                        normal = -normal;
                        d = -d;
                    }

                    // Check if a similar face already exists.
                    bool duplicate = false;
                    foreach (PlaneData face in faces)
                    {
                        if (Vector3.Dot(face.normal, normal) > 1 - EPS && Mathf.Abs(face.d - d) < EPS)
                        {
                            duplicate = true;
                            break;
                        }
                    }
                    if (!duplicate)
                        faces.Add(new PlaneData(normal, d));
                }
            }
        }
        return faces;
    }

    // Given three planes, compute their intersection point if it exists.
    // Returns null if the planes do not have a unique intersection.
    private static Vector3? IntersectThreePlanes(PlaneData a, PlaneData b, PlaneData c)
    {
        // The intersection point is given by:
        // P = (-d_a (b.normal x c.normal) - d_b (c.normal x a.normal) - d_c (a.normal x b.normal))
        //       / (a.normal · (b.normal x c.normal))
        Vector3 cross = Vector3.Cross(b.normal, c.normal);
        float denom = Vector3.Dot(a.normal, cross);
        if (Mathf.Abs(denom) < EPS)
            return null; // The planes are nearly parallel.

        Vector3 term1 = (-a.d) * cross;
        Vector3 term2 = (-b.d) * Vector3.Cross(c.normal, a.normal);
        Vector3 term3 = (-c.d) * Vector3.Cross(a.normal, b.normal);
        Vector3 p = (term1 + term2 + term3) / denom;
        return p;
    }
}