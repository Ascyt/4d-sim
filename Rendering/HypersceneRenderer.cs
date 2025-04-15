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
public class HypersceneRenderer : MonoBehaviour
{
    [System.Serializable]
    public enum HypersceneOption
    {
        Default
    }

    public static HypersceneRenderer instance { get; private set; }

    [SerializeField]
    public HypersceneOption hypersceneOption = HypersceneOption.Default;
    private readonly Dictionary<HypersceneOption, Hyperscene> hyperscenes = new()
    {
        { HypersceneOption.Default, new DefaultHyperscene() }
    };
    public Hyperscene Hyperscene => hyperscenes[hypersceneOption];

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
        objects.AddRange(Hyperscene.Objects);

        if (Hyperscene.ShowFixedAxes)
        {
            fixedAxes = new Axes();
        }

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

        UpdateFixedAxes(Rotation4.zero);
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

        UpdateFixedAxes(rotationDelta);
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

        // Project the fixed axes
        foreach (ConnectedVertices connectedVertices in fixedAxes.vertices)
        {
            connectedVertices.transformedVertices = connectedVertices.vertices
                .Select(v => v.RotateNeg(rotation))
                .ToArray();
            rendering.ProjectFixedObject(connectedVertices, fixedAxes, connectedVertices.transformedVertices);
        }
    }

    private void UpdateFixedAxes(Rotation4 rotationDelta)
    {
        foreach (ConnectedVertices connectedVertices in fixedAxes.vertices)
        {
            connectedVertices.transformedVertices = connectedVertices.transformedVertices
                .Select(v => v.RotateNeg(rotationDelta))
                .ToArray();

            rendering.ProjectFixedObject(connectedVertices, fixedAxes, connectedVertices.transformedVertices);
        }
    }
}
