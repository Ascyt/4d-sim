using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(CameraPosition))]
[RequireComponent(typeof(CameraRotation))]
[RequireComponent(typeof(CameraState))]
[RequireComponent(typeof(Rendering))]
public class HypersceneRenderer : MonoBehaviour
{
    [System.Serializable]
    public enum HypersceneOption
    {
        Default,
        Ground,
        FixedTesseract,
        FixedCubes,
        FixedSingleCube
    }

    public static HypersceneRenderer instance { get; private set; }

    [SerializeField]
    public HypersceneOption hypersceneOption { get; private set; } = HypersceneOption.Default;
    public Hyperscene hyperscene { get; private set; } = null;

    public readonly List<Hyperobject> objects = new();
    public readonly List<Hyperobject> fixedObjects = new();

    private CameraPosition cameraPosition;
    private CameraRotation cameraRotation;
    private CameraState cameraState;
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

        cameraPosition = GetComponent<CameraPosition>();
        cameraRotation = GetComponent<CameraRotation>();
        cameraState = GetComponent<CameraState>();
    }
    private void Start()
    {
        InitializeHyperscene();
    }

    public void LoadHyperscene(HypersceneOption hypersceneOption)
    {
        rendering.ClearAllRenderedObjects();
        objects.Clear();
        fixedObjects.Clear();

        this.hypersceneOption = hypersceneOption;

        InitializeHyperscene();
    }

    private void InitializeHyperscene()
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
            case HypersceneOption.FixedSingleCube:
                hyperscene = new FixedSingleCubeHyperscene();
                break;
            default:
                Debug.LogError("HypersceneRenderer: Unknown hyperscene option.");
                break;
        }

        cameraState.SetPosition(hyperscene.StartingPosition);
        cameraState.SetRotation(hyperscene.StartingRotation);

        objects.AddRange(hyperscene.Objects);
        fixedObjects.AddRange(hyperscene.FixedObjects);

        if (hyperscene.IsFixed && objects.Count > 0)
        {
            Debug.LogWarning("HypersceneRenderer: Fixed hyperscenes should not have any objects.");
        }

        cameraPosition.enabled = !hyperscene.IsFixed;
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
        Vector4 origin = cameraState.position;
        Rotation4 rotation = cameraState.rotation;

        foreach (Hyperobject obj in objects)
        {
            Vector4 pos = obj.position - origin;

            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
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
    public void RerenderObjects()
    {
        rendering.ClearAllRenderedObjects();

        foreach (Hyperobject obj in objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                rendering.ProjectVertices(connectedVertices, obj, connectedVertices.transformedVertices);
            }
        }

        UpdateFixedObjects(Rotation4.zero);
    }

    public void ResetRotation()
    {
        rendering.ClearAllRenderedObjects();
        RenderObjectsInitially();
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
