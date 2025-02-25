using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Tetrahedron : MonoBehaviour
{
    private Vector3[] vertices = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(-1, -1, 1),
        new Vector3(-1, 1, -1),
        new Vector3(1, -1, -1)
    };

    private int[] triangles = new int[]
    {
        0, 2, 1, // First triangle
        0, 3, 2, // Second triangle
        0, 1, 3, // Third triangle
        1, 2, 3  // Fourth triangle
    };

    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = "Tetrahedron";

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
