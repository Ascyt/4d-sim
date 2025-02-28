using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TetrahedronInstantiater : MonoBehaviour
{
    public static TetrahedronInstantiater instance { get; private set; }

    [SerializeField]
    private Material tetrahedronMaterial;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Tetrahedron[] t = new Tesseract().hypermesh;
        for (int i = 0; i < t.Length; i++)
        {
            Vector3 delta = new Vector3(t[i].vertices.Select(v => v.w).Average(), 0, 0);
            InstantiateTetrahedron(t[i], delta * 0f);
        }
    }

    private int[] triangles = new int[]
    {
        0, 1, 2,
        0, 3, 1,
        1, 3, 2,
        2, 3, 0
    };

    public GameObject InstantiateTetrahedron(Tetrahedron tetrahedron, Vector3 position)
    {
        // TODO: Use perspective projection https://hollasch.github.io/ray4/Four-Space_Visualization_of_4D_Objects.html#chapter4
        Vector3[] vertices = tetrahedron.vertices.Select(v => new Vector3(v.x, v.y, v.z)).ToArray();

        Vector3[] uniqueVertices = GetUniqueVertices(vertices);

        GameObject newGameObject = new GameObject($"Tetrahedron");

        newGameObject.transform.position = position;

        MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;


        if (uniqueVertices.Length == 3)
        {
            // Create a triangle
            mesh.triangles = new int[] { 0, 1, 2 };
        }
        else
        {
            mesh.triangles = triangles;
        }

        mesh.RecalculateNormals();
        /*
        // Check if normals are facing upwards, flip if necessary
        if (Vector3.Dot(mesh.normals[0], Vector3.up) < 0)
        {
            FlipTriangles(mesh);
        }

        mesh.RecalculateBounds();
        */
        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(tetrahedronMaterial);
        meshRenderer.material.color = Color.white;

        return newGameObject;
    }

    private static Vector3[] GetUniqueVertices(Vector3[] verts)
    {
        // Use a list to store unique vertices
        List<Vector3> uniqueVerts = new List<Vector3>();

        foreach (var vert in verts)
        {
            if (!uniqueVerts.Contains(vert))
            {
                uniqueVerts.Add(vert);
            }
        }

        return uniqueVerts.ToArray();
    }

    private static void FlipTriangles(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Swap two indices to reverse winding order
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
