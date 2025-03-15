using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Hyperscene : MonoBehaviour
{
    public static Hyperscene instance { get; private set; }

    [SerializeField]
    private Material material;

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
        objects.Add(new Axes(new Material(material)));

        //objects.Add(new Point(new Vector4(0, 0, 0, 0), Color.white));

        objects.Add(new Tesseract(new Vector4(2, 0, 1, 2), ConnectedVertices.ConnectionMethod.Solid, Color.cyan, new Material(material)));
        objects.Add(new Cube(new Vector4(0, -1, 2, 4), ConnectedVertices.ConnectionMethod.Solid, Color.yellow, new Material(material)));

        objects.Add(new Tesseract(new Vector4(3, 0, -2, 3), ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta, new Material(material)));

        objects.Add(new Cube(new Vector4(0, -1, 2, -4), ConnectedVertices.ConnectionMethod.Wireframe, Color.green, new Material(material)));


        fixedAxes = new Axes(new Material(material));

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
                    .Select(v => (v + pos).RotateNeg(rotation))
                    .ToArray();

                Vector4?[] verticesRelativeToCameraNullable = new Vector4?[verticesRelativeToCamera.Length];
                for (int i = 0; i < verticesRelativeToCamera.Length; i++)
                {
                    verticesRelativeToCameraNullable[i] = verticesRelativeToCamera[i].w > 0 ? verticesRelativeToCamera[i] : null;
                }

                if (verticesRelativeToCameraNullable.All(v => !v.HasValue))
                {
                    continue;
                }

                // Project the vertices to 3D
                Vector3?[] transformedVertices = Helpers.ProjectVerticesTo3d(wa, wb, wc, wd, from, verticesRelativeToCameraNullable, angle);

                if (verticesRelativeToCameraNullable.All(v => !v.HasValue))
                {
                    continue;
                }

                Vector3 averagePos = new Vector3(
                    transformedVertices.Where(v => v.HasValue).Select(v => v.Value.x).Average(), 
                    transformedVertices.Where(v => v.HasValue).Select(v => v.Value.y).Average(),
                    transformedVertices.Where(v => v.HasValue).Select(v => v.Value.z).Average());

                transformedVertices = transformedVertices.Select(v => v - averagePos).ToArray();

                GameObject instance = ObjectInstantiator.instance
                    .InstantiateObject(transformedVertices, averagePos, connectedVertices.connectionMethod, connectedVertices.color, connectedVertices.material, connectedVertices.connections, connectedVertices.vertexScale);

                if (instance is not null)
                {
                    instantiatedObjects.Add(instance);
                }
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
                .Select(v => v.RotateNeg(rotation))
                .ToArray();

            Vector3[] transformedVertices = rotatedVertices
                .Select(v => new Vector3(v.x, v.y, v.z)) // orthographic projection by cutting away w coordinate
                .ToArray();

            Vector3?[] transformedVerticesNullable = new Vector3?[transformedVertices.Length];
            for (int i = 0; i < transformedVertices.Length; i++)
            {
                transformedVerticesNullable[i] = transformedVertices[i];
            }

            GameObject instance = ObjectInstantiator.instance
                .InstantiateObject(transformedVerticesNullable, Vector3.zero, connectedVertices.connectionMethod, connectedVertices.color, connectedVertices.material, connectedVertices.connections, connectedVertices.vertexScale);

            if (instance is not null)
            {
                instantiatedObjects.Add(instance);
            }
        }
    }
}
