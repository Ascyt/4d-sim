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
            InstantiateTetrahedron(t[i], new Vector3(t[i].vertices.Select(v => v.w * 10f).Average(), 0, 0));
        }
    }

    private int[] triangles = new int[]
    {
        0, 2, 1, // First triangle
        0, 3, 2, // Second triangle
        0, 1, 3, // Third triangle
        1, 2, 3  // Fourth triangle
    };

    public GameObject InstantiateTetrahedron(Tetrahedron tetrahedron, Vector3 position)
    {
        GameObject newGameObject = new GameObject($"Tetrahedron");

        newGameObject.transform.position = position;

        MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newGameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.vertices = tetrahedron.vertices.Select(v => new Vector3(v.x, v.y, v.z)).ToArray();
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
        meshRenderer.material = new Material(tetrahedronMaterial);
        meshRenderer.material.color = Color.black;

        return newGameObject;
    }
}
