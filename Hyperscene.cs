using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

[RequireComponent(typeof(CameraMovement))]
[RequireComponent(typeof(CameraRotation))]
[RequireComponent(typeof(CameraPosition))]
[RequireComponent(typeof(Rendering))]
public class Hyperscene : MonoBehaviour
{
    public static Hyperscene instance { get; private set; }

    public readonly List<Hyperobject> objects = new();
    public Hyperobject fixedAxes;

    private CameraMovement cameraMovement;
    private CameraRotation cameraRotation;
    private CameraPosition cameraPosition;
    private Rendering rendering;

    private readonly float fov = Mathf.PI / 4f;
    private readonly Vector4 from = new Vector4(0, 0, 0, 0);
    private readonly Vector4 to = new Vector4(0, 0, 0, -1);
    private readonly Vector4 up = new Vector4(0, 1, 0, 0);
    private readonly Vector4 over = new Vector4(0, 0, 1, 0);

    private void Awake()
    {
        instance = this;

        rendering = GetComponent<Rendering>();
        rendering.Initialize(from, to, up, over, fov);

        cameraMovement = GetComponent<CameraMovement>();
        cameraRotation = GetComponent<CameraRotation>();
        cameraPosition = GetComponent<CameraPosition>();

        cameraMovement.onPositionUpdate += RenderObjectsCameraPositionChange;
        cameraRotation.onRotationUpdate += RenderObjectsCameraRotationChange;
    }
    private void Start()
    {
        objects.Add(new Axes());
        
        objects.Add(new Point(new Vector4(0, 0, 0, 0), Color.white));

        objects.Add(new Tesseract(new Vector4(2, 0, 1, 2), ConnectedVertices.ConnectionMethod.Solid, Color.cyan, 1f));
        objects.Add(new Cube(new Vector4(0, -1, 2, 4), ConnectedVertices.ConnectionMethod.Solid, Color.yellow));
        
        objects.Add(new Tesseract(new Vector4(3, 0, -2, 3), ConnectedVertices.ConnectionMethod.Wireframe, Color.magenta, 1f));
        
        objects.Add(new Cube(new Vector4(0, -1, 2, -4), ConnectedVertices.ConnectionMethod.Wireframe, Color.green));
        
        objects.Add(new Tesseract(new Vector4(-10, 5, 6, -3), ConnectedVertices.ConnectionMethod.Wireframe, Color.red, 3f));
        objects.Add(new Tesseract(new Vector4(-10, 5, 6, -7), ConnectedVertices.ConnectionMethod.Wireframe, Color.yellow, .5f));

        //fixedAxes = new Axes();

        RenderObjectsInitially();
    }

    public void RenderObjectsCameraPositionChange(Vector4 positionDelta)
    {
        rendering.ClearAllRenderedObjects();

        foreach (Hyperobject obj in objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                connectedVertices.transformedVertices = connectedVertices.transformedVertices
                    .Select(v => v - positionDelta)
                    .ToArray();

                rendering.ProjectVertices(connectedVertices, obj, connectedVertices.transformedVertices);
            }
        }
    }
    public void RenderObjectsCameraRotationChange(Rotation4 rotationDelta)
    {
        rendering.ClearAllRenderedObjects();

        foreach (Hyperobject obj in objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                connectedVertices.transformedVertices = connectedVertices.transformedVertices
                    .Select(v => v.RotateNeg(rotationDelta))
                    .ToArray();

                rendering.ProjectVertices(connectedVertices, obj, connectedVertices.transformedVertices);
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

                rendering.ProjectVertices(connectedVertices, obj, verticesRelativeToCamera);
            }
        }

        RenderFixedAxes();
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
