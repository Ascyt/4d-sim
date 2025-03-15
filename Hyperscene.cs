using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class Hyperscene : MonoBehaviour
{
    public static Hyperscene instance { get; private set; }

    [SerializeField]
    private Material material;

    public readonly List<Hyperobject> objects = new();
    public Hyperobject fixedAxes;

    private List<GameObject> instantiatedObjects = new();
    private List<Object> instantiatedObjectsResources = new();

    private CameraMovement cameraMovement;
    private CameraRotation cameraRotation;
    private CameraPosition cameraPosition;

    private readonly float fov = Mathf.PI / 4f;
    private readonly Vector4 from = new Vector4(0, 0, 0, 0);
    private readonly Vector4 to = new Vector4(0, 0, 0, -1);
    private readonly Vector4 up = new Vector4(0, 1, 0, 0);
    private readonly Vector4 over = new Vector4(0, 0, 1, 0);
    private Vector4 wa;
    private Vector4 wb;
    private Vector4 wc;
    private Vector4 wd;

    private void Awake()
    {
        instance = this;
        cameraMovement = GetComponent<CameraMovement>();
        cameraRotation = GetComponent<CameraRotation>();
        cameraPosition = GetComponent<CameraPosition>();

        Helpers.GetViewingTransformMatrix(from, to, up, over, out wa, out wb, out wc, out wd);

        cameraMovement.onPositionUpdate += RenderObjectsCameraPositionChange;
        cameraRotation.onRotationUpdate += RenderObjectsCameraRotationChange;
    }
    private void Start()
    {
        objects.Add(new Axes(new Material(material)));
        
        objects.Add(new Point(new Vector4(0, 0, 0, 0), Color.white, new Material(material)));

        objects.Add(new Tesseract(new Vector4(2, 0, 1, 2), ConnectedVertices.ConnectionMethod.Solid, Color.cyan, new Material(material), 1f));
        objects.Add(new Cube(new Vector4(0, -1, 2, 4), ConnectedVertices.ConnectionMethod.Solid, Color.yellow, new Material(material)));
        
        objects.Add(new Tesseract(new Vector4(3, 0, -2, 3), ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta, new Material(material), 1f));
        
        objects.Add(new Cube(new Vector4(0, -1, 2, -4), ConnectedVertices.ConnectionMethod.Wireframe, Color.green, new Material(material)));
        
        objects.Add(new Tesseract(new Vector4(-10, 5, 6, -3), ConnectedVertices.ConnectionMethod.Wireframe, Color.red, new Material(material), 3f));
        objects.Add(new Tesseract(new Vector4(-10, 5, 6, -7), ConnectedVertices.ConnectionMethod.Wireframe, Color.yellow, new Material(material), .5f));

        //fixedAxes = new Axes(new Material(material));

        RenderObjectsInitially();
    }

    public void RenderObjectsCameraPositionChange(Vector4 positionDelta)
    {
        ClearRenderedObjects();

        foreach (Hyperobject obj in objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                connectedVertices.transformedVertices = connectedVertices.transformedVertices
                    .Select(v => v - positionDelta)
                    .ToArray();

                ProjectVertices(connectedVertices, connectedVertices.transformedVertices);
            }
        }
    }
    public void RenderObjectsCameraRotationChange(Rotation4 rotationDelta)
    {
        ClearRenderedObjects();

        foreach (Hyperobject obj in objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                connectedVertices.transformedVertices = connectedVertices.transformedVertices
                    .Select(v => v.RotateNeg(rotationDelta))
                    .ToArray();

                ProjectVertices(connectedVertices, connectedVertices.transformedVertices);
            }
        }
    }

    public void RenderObjectsInitially()
    {
        Vector4 origin = cameraPosition.position;
        Rotation4 rotation = cameraPosition.rotation;

        foreach (Hyperobject obj in objects) 
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                Vector4 pos = obj.position - origin;

                // Transform vertices so camera rotation and position is 0
                Vector4[] verticesRelativeToCamera = connectedVertices.vertices
                    .Select(v => (v + pos).RotateNeg(rotation))
                    .ToArray();

                connectedVertices.transformedVertices = verticesRelativeToCamera;

                ProjectVertices(connectedVertices, verticesRelativeToCamera);
            }
        }

        RenderFixedAxes();
    }

    private void ClearRenderedObjects()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();

        foreach (Object resource in instantiatedObjectsResources)
        {
            Destroy(resource);
        }
        instantiatedObjectsResources.Clear();
    }

    private void ProjectVertices(ConnectedVertices connectedVertices, Vector4[] verticesRelativeToCamera)
    {
        Vector4?[] verticesRelativeToCameraNullable = new Vector4?[verticesRelativeToCamera.Length];
        for (int i = 0; i < verticesRelativeToCamera.Length; i++)
        {
            verticesRelativeToCameraNullable[i] = verticesRelativeToCamera[i].w > 0 ? verticesRelativeToCamera[i] : null;
        }

        if (verticesRelativeToCameraNullable.All(v => !v.HasValue))
        {
            return;
        }

        // Project the vertices to 3D
        Vector3?[] transformedVertices = Helpers.ProjectVerticesTo3d(wa, wb, wc, wd, from, verticesRelativeToCameraNullable, fov);

        if (verticesRelativeToCameraNullable.All(v => !v.HasValue))
        {
            return;
        }

        Vector3 averagePos = new Vector3(
            transformedVertices.Where(v => v.HasValue).Select(v => v.Value.x).Average(),
            transformedVertices.Where(v => v.HasValue).Select(v => v.Value.y).Average(),
            transformedVertices.Where(v => v.HasValue).Select(v => v.Value.z).Average());

        transformedVertices = transformedVertices.Select(v => v - averagePos).ToArray();
        (GameObject, List<Object>)? instance = ObjectInstantiator.instance
          .InstantiateObject(transformedVertices, averagePos, connectedVertices.connectionMethod, connectedVertices.color, connectedVertices.material, connectedVertices.connections, connectedVertices.vertexScale);

        if (instance != null)
        {
            instantiatedObjects.Add(instance.Value.Item1);
            instantiatedObjectsResources.AddRange(instance.Value.Item2);
        }
    }

    private void RenderFixedAxes()
    {
        return;
        Rotation4 rotation = cameraPosition.rotation;

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

            //GameObject instance = ObjectInstantiator.instance
             //   .InstantiateObject(transformedVerticesNullable, Vector3.zero, connectedVertices.connectionMethod, connectedVertices.color, connectedVertices.material, connectedVertices.connections, connectedVertices.vertexScale);

            if (instance != null)
            {
               // instantiatedObjects.Add(instance);
            }
        }
    }
}
