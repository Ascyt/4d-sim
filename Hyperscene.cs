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
    public Hyperobject fixedAxes;

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
        objects.Add(new Axes());

        //objects.Add(new Point(new Vector4(0, 0, 0, 0), Color.white));

        objects.Add(new Tesseract(new Vector4(2, 0, 1, 2), ConnectedVertices.ConnectionMethod.Solid, Color.cyan));
        objects.Add(new Cube(new Vector4(0, -1, 2, 4), ConnectedVertices.ConnectionMethod.Solid, Color.yellow));

        objects.Add(new Tesseract(new Vector4(3, 0, -2, 3), ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta));

        fixedAxes = new Axes();

        RenderObjects();
    }

    public void RenderObjects()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();

        Vector4 origin = cameraPos.position;
        Rotation4 rotation = cameraPos.rotation;

        Vector4 from = new Vector4(0, 0, 0, 0);
        Vector4 to = new Vector4(0, 0, 0, -1);
        Vector4 up = new Vector4(0, 1, 0, 0);
        Vector4 over = new Vector4(0, 0, 1, 0);

        Helpers.GetViewingTransformMatrix(from, to, up, over, out Vector4 wa, out Vector4 wb, out Vector4 wc, out Vector4 wd);

        float angle = Mathf.PI / 4f;

        foreach (Hyperobject obj in objects) 
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                Vector4 pos = obj.position - origin;

                // Transform vertices so camera rotation and position is 0
                Vector4[] verticesRelativeToCamera = connectedVertices.vertices
                    .Select(v => (v + pos).Rotate(-rotation))
                    .ToArray();

                if (verticesRelativeToCamera.Any(v => v.w < 0))
                    continue;

                // Project the vertices to 3D
                Vector3[] transformedVertices = Helpers.ProjectVerticesTo3d(wa, wb, wc, wd, from, verticesRelativeToCamera, angle);

                Vector3 averagePos = new Vector3(
                    transformedVertices.Select(v => v.x).Average(), 
                    transformedVertices.Select(v => v.y).Average(),
                    transformedVertices.Select(v => v.z).Average());

                transformedVertices = transformedVertices.Select(v => v - averagePos).ToArray();

                GameObject instance = ObjectInstantiator.instance
                    .InstantiateObject(transformedVertices, averagePos, connectedVertices.connectionMethod, connectedVertices.color, connectedVertices.connections, connectedVertices.vertexScale);

                if (instance != null)
                    instantiatedObjects.Add(instance);
            }
        }

        RenderFixedAxes();
    }

    private void RenderFixedAxes()
    {
        Rotation4 rotation = cameraPos.rotation;

        foreach (ConnectedVertices connectedVertices in fixedAxes.vertices)
        {
            Vector4[] rotatedVertices = connectedVertices.vertices
                .Select(v => v.Rotate(-rotation))
                .ToArray();

            Vector3[] transformedVertices = rotatedVertices
                .Select(v => new Vector3(v.x, v.y, v.z)) // orthographic projection by cutting away w coordinate
                .ToArray();

            GameObject instance = ObjectInstantiator.instance
                .InstantiateObject(transformedVertices, Vector3.zero, connectedVertices.connectionMethod, connectedVertices.color, connectedVertices.connections, connectedVertices.vertexScale);

            if (instance != null)
                instantiatedObjects.Add(instance);
        }
    }
}
