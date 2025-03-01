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

    private int[] triangles = new int[]
    {
        0, 1, 2,
        0, 3, 1,
        1, 3, 2,
        2, 3, 0
    };

    public GameObject InstantiateTetrahedron(Vector3[] vertices, Vector4 position)
    {
        Vector3[] filteredVertices = RemoveFaultyVertices(GetUniqueVertices(vertices), position);

        GameObject newGameObject = new GameObject("Tetrahedron");

        newGameObject.transform.position = position;

        MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;


        if (filteredVertices.Length <= 2)
        {
            return null;
        }
        else if (filteredVertices.Length == 3)
        {
            // Create a triangle
            mesh.triangles = new int[] { 0, 1, 2 };
        }
        else
        {
            mesh.triangles = triangles;
        }

        mesh.RecalculateNormals();
        
        // Check if normals are facing upwards, flip if necessary
        if (Vector3.Dot(mesh.normals[0], Vector3.up) < 0)
        {
            FlipTriangles(mesh);
        }

        mesh.RecalculateBounds();
        
        meshFilter.mesh = mesh;
        meshRenderer.material = tetrahedronMaterial;
        meshRenderer.material.color = Color.white;

        return newGameObject;
    }

    private static Vector3[] GetUniqueVertices(Vector3[] verts)
    {
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
    private static Vector3[] RemoveFaultyVertices(Vector3[] verts, Vector3 position)
    {
        List<Vector3> output = new List<Vector3>();
        foreach (var vert in verts)
        {
            Vector3 pos = vert + position;

            if ((!float.IsNaN(vert.x) && !float.IsNaN(vert.y) && !float.IsNaN(vert.z)) &&
                (!float.IsInfinity(vert.x) && !float.IsInfinity(vert.y) && !float.IsInfinity(vert.z)) &&
                (pos.x > -10 && pos.x < 10 && pos.y > -10 && pos.y < 10 && pos.z > -10 && pos.z < 10))
            {
                output.Add(vert);
            }
        }
        return output.ToArray();
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
