using MIConvexHull;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Instantiates the 3D objects projected from 4D vertices.<br />
/// </summary>
public class ObjectInstantiator : MonoBehaviour
{
    public struct InstantiatedObject
    {
        public GameObject gameObj;
        public List<Object> resources;
        public InstantiatedObject(GameObject gameObj, List<Object> resources)
        {
            this.gameObj = gameObj; this.resources = resources;
        }
    }

    public static ObjectInstantiator instance { get; private set; }
    [SerializeField]
    private Material solidMaterial;
    [SerializeField]
    private Material wireframeLineMaterial;
    [SerializeField]
    private Material wireframeVertexMaterial;

    private void Awake()
    {
        instance = this;
    }

    public InstantiatedObject? InstantiateObject(Vector3?[] points, Vector3 position, ConnectedVertices.ConnectionMethod connectionMethod, Color color, int[][] connections=null, float? vertexScale=null)
    {
        switch (connectionMethod)
        {
            case ConnectedVertices.ConnectionMethod.Solid:
                return InstantiateObjectSolid(points, position, color);
            case ConnectedVertices.ConnectionMethod.Wireframe:
                return InstantiateObjectWireframe(points, position, color, connections, vertexScale);
            case ConnectedVertices.ConnectionMethod.Vertices:
                return InstantiateObjectVertices(points, position, color, vertexScale);
        }

        Debug.LogError("Unknown ConnectionMethod");
        return null;
    }

    /// <summary>
    /// Instantiates a solid object using the convex hull of the given points. Utilizes <a href="https://designengrlab.github.io/MIConvexHull/">MIConvexHull</a>.
    /// </summary>
    private InstantiatedObject? InstantiateObjectSolid(Vector3?[] points, Vector3 position, Color color)
    {
        List<Object> resources = new();

        Vector3[] pointsValues = points.Where(p => p.HasValue).Select(p => p.Value).ToArray();

        pointsValues = VertexClipper.ClipVertices(pointsValues, position);

        if (pointsValues.Length < 4)
        {
            return null;
        }

        // Convert Unity Vector3 points to MIVertex objects.
        MIVertex[] vertices = pointsValues.Select(p => new MIVertex(p)).ToArray();
        
        // Compute the convex hull using MIConvexHull.
        // The returned hull contains Faces, where each face has a set of vertices.
        var convexHullResult = ConvexHull.Create<MIVertex, DefaultConvexFace<MIVertex>>(vertices);

        if (convexHullResult.Outcome != ConvexHullCreationResultOutcome.Success)
        {
            //Debug.LogError("Convex hull generation failed: " + convexHullResult.ErrorMessage);
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

        resources.Add(mesh);

        // Create a GameObject and assign the mesh.
        GameObject obj = new GameObject("ConvexHullObject");

        obj.transform.position = position;

        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        mf.mesh = mesh;

        Material mat = new Material(solidMaterial);
        mat.color = color;

        mr.material = mat;

        resources.Add(mat);

        return new InstantiatedObject(obj, resources);
    }


    /// <summary>
    /// Creates a parent GameObject with sphere children placed at the given vertex positions.
    /// </summary>
    private InstantiatedObject? InstantiateObjectVertices(Vector3?[] points, Vector3 position, Color color, float? vertexScale)
    {
        if (points.Length == 0 || points.All(points => !points.HasValue))
        {
            return null;
        }

        List<Object> resources = new();

        GameObject parent = new GameObject("VertexObject");
        parent.transform.position = position;

        Material mat = new Material(wireframeVertexMaterial);
        mat.color = color;

        foreach (Vector3? pt in points)
        {
            if (!pt.HasValue)
            {
                continue;
            }

            GameObject vertexSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            vertexSphere.transform.parent = parent.transform;
            vertexSphere.transform.localPosition = pt.Value;
            vertexSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f) * (vertexScale ?? 1f);

            MeshRenderer mr = vertexSphere.GetComponent<MeshRenderer>();

            mr.material = mat;

            resources.Add(mr.material);
        }
        resources.Add(mat);

        return new InstantiatedObject(parent, resources);
    }

    /// <summary>
    /// Creates the wireframe object that connects every vertex to every other vertex. <br /><br />
    /// 
    /// This method creates a wireframe by instantiating vertices and connecting specific vertices. <br />
    /// </summary>
    /// <param name="connectedVertices">
    /// Array of int[2], where each pair is the indices of vertices to connect.
    /// </param>

    private InstantiatedObject? InstantiateObjectWireframe(Vector3?[] points, Vector3 position, Color color, int[][] connectedVertices, float? vertexScale)
    {
        List<Object> resources = new();

        // Create the basic vertex GameObjects.
        InstantiatedObject? verticesObject = InstantiateObjectVertices(points, position, color, vertexScale);

        resources.AddRange(verticesObject.Value.resources);

        if (verticesObject is null)
            return null;

        verticesObject.Value.gameObj.name = "WireframeObject";

        Material mat = new Material(wireframeLineMaterial);
        mat.color = color;

        // For each connection, create a child GameObject with a LineRenderer.
        for (int i = 0; i < connectedVertices.Length; i++)
        {
            if (connectedVertices[i][0] >= points.Length || connectedVertices[i][1] >= points.Length)
            {
                continue;
            }
            if (!points[connectedVertices[i][0]].HasValue || !points[connectedVertices[i][1]].HasValue)
            {
                continue;
            }

            // Create a child GameObject for the line segment.
            GameObject lineObject = new GameObject("WireframeLine_" + i);
            // Set as child of the parent,
            // so that the line positions can be specified in local space.
            lineObject.transform.parent = verticesObject.Value.gameObj.transform;
            lineObject.transform.localPosition = Vector3.zero;

            // Add and configure the LineRenderer.
            LineRenderer lr = lineObject.AddComponent<LineRenderer>();
            lr.useWorldSpace = false; // use local positions to match the spheres.
            lr.positionCount = 2; // Each connection is just 2 points.
            lr.SetPosition(0, points[connectedVertices[i][0]].Value);
            lr.SetPosition(1, points[connectedVertices[i][1]].Value);

            // Set width and material.
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.material = mat;
        }

        resources.Add(mat);

        return new InstantiatedObject(verticesObject.Value.gameObj, resources);
    }
}