using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
        Tesseract t = new Tesseract(new Vector4(0, 0, 0, 2));

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
        Vector4 over = from + new Vector4(0, 0, 1, 0);
        Helpers.GetViewingTransformMatrix(from, to, up, over, out Vector4 wa, out Vector4 wb, out Vector4 wc, out Vector4 wd);

        float angle = Mathf.PI / 4f;

        foreach (Hyperobject obj in objects) 
        {
            foreach (Tetrahedron t in obj.hypermesh)
            {
                Vector3[] transformedVertices = Helpers.ProjectVerticesTo3d(wa, wb, wc, wd, from, t.vertices, obj.position, angle);

                Vector3 averagePos = new Vector3(
                    transformedVertices.Select(v => v.x).Average(), 
                    transformedVertices.Select(v => v.y).Average(),
                    transformedVertices.Select(v => v.z).Average());

                transformedVertices = transformedVertices.Select(v => v - averagePos).ToArray();

                GameObject instance = TetrahedronInstantiater.instance.InstantiateTetrahedron(transformedVertices, averagePos);

                if (instance != null)
                    instantiatedObjects.Add(instance);
            }
        }
    }
}
