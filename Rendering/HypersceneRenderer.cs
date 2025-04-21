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
        Default,
        Ground,
        FixedTesseract,
        FixedCubes
    }

    public static HypersceneRenderer instance { get; private set; }

    [SerializeField]
    public HypersceneOption hypersceneOption = HypersceneOption.Default;
    public Hyperscene hyperscene;

    public readonly List<Hyperobject> objects = new();
    public readonly List<Hyperobject> fixedObjects = new();

    private CameraMovement cameraMovement;
    private CameraRotation cameraRotation;
    private CameraPosition cameraPosition;
    private Rendering rendering;

    private readonly float fov = Mathf.PI / 8f;
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
        InitializeHyperscene();
    }

    public void InitializeHyperscene()
    {
        switch (hypersceneOption)
        {
            case HypersceneOption.Default:
                hyperscene = new DefaultHyperscene();
                break;
            case HypersceneOption.Ground:
                hyperscene = new GroundHyperscene();
                break;
            case HypersceneOption.FixedTesseract:
                hyperscene = new FixedTesseractHyperscene();
                break;
            case HypersceneOption.FixedCubes:
                hyperscene = new FixedCubesHyperscene();
                break;
            default:
                Debug.LogError("HypersceneRenderer: Unknown hyperscene option.");
                break;
        }

        objects.AddRange(hyperscene.Objects);
        fixedObjects.AddRange(hyperscene.FixedObjects);

        if (hyperscene.IsFixed && objects.Count > 0)
        {
            Debug.LogWarning("HypersceneRenderer: Fixed hyperscenes should not have any objects.");
        }

        cameraMovement.enabled = !hyperscene.IsFixed;
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

        UpdateFixedObjects(Rotation4.zero);
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

        UpdateFixedObjects(rotationDelta);
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

        // Project the fixed objects
        foreach (Hyperobject fixedObject in fixedObjects)
        {
            foreach (ConnectedVertices connectedVertices in fixedObject.vertices)
            {
                connectedVertices.transformedVertices = connectedVertices.vertices
                    .Select(v => (v + fixedObject.position).RotateNeg(rotation))
                    .ToArray();

                rendering.ProjectFixedObject(connectedVertices, fixedObject, connectedVertices.transformedVertices, !hyperscene.IsFixed || hyperscene.IsOrthographic);
            }
        }
    }

    private void UpdateFixedObjects(Rotation4 rotationDelta)
    {
        foreach (Hyperobject fixedObject in fixedObjects)
        {
            foreach (ConnectedVertices connectedVertices in fixedObject.vertices)
            {
                connectedVertices.transformedVertices = connectedVertices.transformedVertices
                    .Select(v => hyperscene.IsFixed ? v.Rotate(rotationDelta) : v.RotateNeg(rotationDelta))
                    .ToArray();

                rendering.ProjectFixedObject(connectedVertices, fixedObject, connectedVertices.transformedVertices, !hyperscene.IsFixed || hyperscene.IsOrthographic);
            }
        }
    }
}
