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
    private Vector4 from = new Vector4(0, 0, 0, 0);
    private Vector4 to = new Vector4(0, 0, 0, -1);
    private Vector4 up = new Vector4(0, 1, 0, 0);
    private Vector4 over = new Vector4(0, 0, 1, 0);

    private void Awake()
    {
        instance = this;

        rendering = GetComponent<Rendering>();
        rendering.Initialize(fov);

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

        fixedAxes = new Axes();

        RenderObjectsInitially();
    }

    public void RenderObjectsCameraPositionChange(Vector4 positionDelta)
    {
        rendering.ClearAllRenderedObjects();

        from += positionDelta;
        to += positionDelta;
        //up += positionDelta;
        //over += positionDelta;

        DrawObjects();

        UpdateFixedAxes(Rotation4.zero);
    }
    public void RenderObjectsCameraRotationChange(Rotation4 rotationDelta)
    {
        rendering.ClearAllRenderedObjects();

        to = to.RotateAroundPoint(from, rotationDelta);
        up = up.Rotate(rotationDelta);
        over = over.Rotate(rotationDelta);

        DrawObjects();

        UpdateFixedAxes(rotationDelta);
    }

    private void DrawObjects()
    {
        Helpers.GetViewingTransformMatrix(from, to, up, over, out Vector4 wa, out Vector4 wb, out Vector4 wc, out Vector4 wd);

        foreach (Hyperobject obj in objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                rendering.ProjectVertices(connectedVertices, obj, from, to, up, over, wa, wb, wc, wd);
            }
        }
    }

    public void RenderObjectsInitially()
    {
        Vector4 origin = cameraPosition.position;
        Rotation4 rotation = cameraPosition.rotation;

        to = to.Rotate(rotation);
        up = up.Rotate(rotation);
        over = over.Rotate(rotation);

        from = origin;
        to += origin;
        up += origin;
        over += origin;

        DrawObjects();

        // Project the fixed axes
        foreach (ConnectedVertices connectedVertices in fixedAxes.vertices)
        {
            rendering.ProjectFixedObject(connectedVertices, fixedAxes);
        }
    }

    private void UpdateFixedAxes(Rotation4 rotationDelta)
    {
        foreach (ConnectedVertices connectedVertices in fixedAxes.vertices)
        {
            rendering.ProjectFixedObject(connectedVertices, fixedAxes);
        }
    }
}
