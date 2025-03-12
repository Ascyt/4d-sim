using MIConvexHull;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ObjectInstantiator : MonoBehaviour
{
    public static ObjectInstantiator instance { get; private set; }

    [SerializeField]
    private Material material;

    private void Awake()
    {
        instance = this;
    }

    public GameObject InstantiateObject(Vector3[] points, Vector3 position)
    {
        if (points == null || points.Length < 4)
        {
            Debug.LogError("At least 4 non-coplanar points are needed to compute a 3D convex hull.");
            return null;
        }
        if (points.Any(p => IsVector3NaNOrInfinity(p)))
        {
            return null;
        }

        // Convert Unity Vector3 points to MIVertex objects.
        MIVertex[] vertices = points.Select(p => new MIVertex(p)).ToArray();

        // Compute the convex hull using MIConvexHull.
        // The returned hull contains Faces, where each face has a set of vertices.

        foreach (var p in points)
            Debug.Log("Point: " + p);

        var convexHullResult = ConvexHull.Create<MIVertex, DefaultConvexFace<MIVertex>>(vertices);

        if (convexHullResult.Outcome != ConvexHullCreationResultOutcome.Success)
        {
            Debug.LogError("Convex hull generation failed: " + convexHullResult.ErrorMessage);
            return null;
        }

        var result = convexHullResult.Result;

        // Prepare lists to build Unity mesh data.
        List<Vector3> meshVertices = new List<Vector3>();
        List<int> meshTriangles = new List<int>();

        // We use a dictionary to map our MIVertex objects to unique indices.
        Dictionary<MIVertex, int> vertexToIndex = new Dictionary<MIVertex, int>();

        // Process each face in the convex hull. Each face is a polygon,
        // but if the hull is truly “convex” in 3D, the faces will be triangles.
        foreach (var face in result.Faces)
        {
            // The face provides its vertices in an array.
            // If the face is not a triangle, you may need additional triangulation.
            MIVertex[] faceVertices = face.Vertices;

            // If face is a polygon with more than 3 vertices, do a simple fan triangulation.
            for (int i = 0; i < faceVertices.Length; i++)
            {
                // Map vertices from MyVertex to mesh indices.
                if (!vertexToIndex.ContainsKey(faceVertices[i]))
                {
                    vertexToIndex[faceVertices[i]] = meshVertices.Count;
                    meshVertices.Add(faceVertices[i].ToVector3());
                }
            }

            // Simple Fan Triangulation for this face.
            for (int i = 1; i < faceVertices.Length - 1; i++)
            {
                meshTriangles.Add(vertexToIndex[faceVertices[0]]);
                meshTriangles.Add(vertexToIndex[faceVertices[i]]);
                meshTriangles.Add(vertexToIndex[faceVertices[i + 1]]);
            }
        }

        // Create the Unity Mesh.
        Mesh mesh = new Mesh();
        mesh.name = "ConvexHullMesh";
        mesh.SetVertices(meshVertices);
        mesh.SetTriangles(meshTriangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Create a GameObject and assign the mesh.
        GameObject obj = new GameObject("ConvexHullObject");
        obj.transform.position = position;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        mf.mesh = mesh;

        mr.material = material;

        return obj;
    }
    bool IsVector3NaNOrInfinity(Vector3 vector)
        => float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z) ||
           float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z);
}
