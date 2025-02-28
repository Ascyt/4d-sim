using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TetrahedronInstance : MonoBehaviour
{
    private Vector3[] vertices = new Vector3[]
    {
        new Vector3(1, 1, 1),
        new Vector3(-1, -1, 1),
        new Vector3(-1, 1, -1),
        new Vector3(1, -1, -1)
    };

    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = "Tetrahedron";

        mesh.vertices = vertices;
        //mesh.triangles = triangles;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
