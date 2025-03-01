using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hyperscene : MonoBehaviour
{
    public static Hyperscene instance { get; private set; }

    public readonly List<Hyperobject> objects = new();
    private List<GameObject> instantiatedObjects = new();

    private CameraPosition cameraPos;

    private void Awake()
    {
        instance = this;
        cameraPos = GetComponent<CameraPosition>();
        cameraPos.onValuesUpdate += RenderObjects;
    }
    private void Start()
    {
        Tesseract t = new Tesseract(new Vector4(0, 0, 0, 1));

        objects.Add(t);
    }

    public void RenderObjects()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();

        Vector4 from = cameraPos.position;
        Vector4 to = from + new Vector4(0, 0, 0, 1);
        Vector4 up = from + new Vector4(0, 1, 0, 0);
        Vector4 over = from + new Vector4(1, 0, 0, 0);
        Matrix4x4 transformationMatrix = Helpers.CreatePerspectiveViewingTransform(from, to, up, over);

        float angle = Mathf.PI / 4f;

        foreach (Hyperobject obj in objects) 
        { 
            Vector3 objPos = Helpers.GetTransformedCoordinate(transformationMatrix, cameraPos.position, obj.position, angle);

            foreach (Tetrahedron t in obj.hypermesh)
            {   
                Vector3[] transformedVertices = new Vector3[t.vertices.Length];
                for (int i = 0; i < t.vertices.Length; i++)
                {
                    Vector4 transformedVertex = Helpers.GetTransformedCoordinate(transformationMatrix, cameraPos.position, t.vertices[i], angle);
                    transformedVertices[i] = new Vector3(transformedVertex.x, transformedVertex.y, transformedVertex.z);
                }

                GameObject instance = TetrahedronInstantiater.instance.InstantiateTetrahedron(transformedVertices, objPos);
                instantiatedObjects.Add(instance);
            }
        }
    }
}
