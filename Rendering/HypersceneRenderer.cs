using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

#nullable enable

/// <summary>
/// Manages the rendering of hyperscenes, including camera position, rotation, and state using Rendering.cs.<br /><br />
/// 
/// In order to solve the rotation order issue of Euler Angles, the actual camera is kept at the origin (0, 0, 0, 0),<br />
/// and the hyperscene is moved and rotated relative to it in the opposite direction. <br />
/// This is to allow the virtual camera to rotate along its own relative planes, rather than world space.
/// </summary>
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
        FixedRotationalPlanes
    }

    public static HypersceneRenderer instance { get; private set; } = null!;

    [SerializeField]
    public HypersceneOption hypersceneOption { get; private set; } = HypersceneOption.Default;
    public Hyperscene? hyperscene { get; private set; } = null;

    public readonly List<Hyperobject> objects = new();
    public readonly List<Hyperobject> fixedObjects = new();

    private CameraPosition cameraPosition = null!;
    private CameraRotation cameraRotation = null!;
    private CameraState cameraState = null!;
    private Rendering rendering = null!;

    private readonly float fov = Mathf.PI / 8f;

    private void Awake()
    {
        instance = this;

        rendering = GetComponent<Rendering>();
        rendering.fov = fov;

        cameraPosition = GetComponent<CameraPosition>();
        cameraRotation = GetComponent<CameraRotation>();
        cameraState = GetComponent<CameraState>();
    }
    private void Start()
    {
        rendering.Initialize();
        InitializeHyperscene();
    }

    private void Update()
    {
        List<Hyperobject>? rerenderObjects = hyperscene!.Update();

        if (rerenderObjects is not null)
        {
            foreach (Hyperobject rerenderObject in rerenderObjects)
            {
                _ = rendering.RemoveSingleObject(rerenderObject);

                foreach (ConnectedVertices connectedVertices in rerenderObject.vertices)
                {
                    rendering.ProjectVertices(connectedVertices, rerenderObject, cameraState.rotation, cameraState.position);
                }
            }
        }
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
            case HypersceneOption.FixedRotationalPlanes:
                hyperscene = new FixedRotationalPlanesHyperscene();
                break;
            default:
                Debug.LogError("HypersceneRenderer: Unknown hyperscene option.");
                break;
        }

        hyperscene!.Start();

        objects.AddRange(hyperscene.Objects);
        fixedObjects.AddRange(hyperscene.FixedObjects);

        if (hyperscene.IsFixed && objects.Count > 0)
        {
            Debug.LogWarning("HypersceneRenderer: Fixed hyperscenes should not have any non-fixed objects.");
        }

        cameraPosition.enabled = !hyperscene.IsFixed;

        RenderObjectsInitially();
    }

    public void RenderObjectsCameraPositionChange()
    {
        rendering.ClearAllRenderedObjects();

        foreach (Hyperobject obj in objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                rendering.ProjectVertices(connectedVertices, obj, cameraState.rotation, cameraState.position);
            }
        }

        UpdateFixedObjects();
    }
    public void RenderObjectsCameraRotationChange()
    {
        rendering.ClearAllRenderedObjects();

        foreach (Hyperobject obj in objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                rendering.ProjectVertices(connectedVertices, obj, cameraState.rotation, cameraState.position);
            }
        }

        UpdateFixedObjects();
    }

    public void RenderObjectsInitially()
    {
        cameraState.SetPosition(hyperscene!.StartingPosition);
        cameraState.SetRotation(hyperscene!.StartingRotation);

        foreach (Hyperobject obj in objects)
        {
            Vector4 pos = obj.position - cameraState.position;

            foreach (ConnectedVertices connectedVertices in obj.vertices)
            {
                rendering.ProjectVertices(connectedVertices, obj, cameraState.rotation, cameraState.position);
            }
        }

        // Project the fixed objects
        foreach (Hyperobject fixedObject in fixedObjects)
        {
            foreach (ConnectedVertices connectedVertices in fixedObject.vertices)
            {
                rendering.ProjectFixedVertices(connectedVertices, fixedObject, cameraState.rotation, !hyperscene.IsFixed || hyperscene.IsOrthographic);
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
                rendering.ProjectVertices(connectedVertices, obj, cameraState.rotation, cameraState.position);
            }
        }

        UpdateFixedObjects();
    }

    public void ResetRotation()
    {
        rendering.ClearAllRenderedObjects();
        RenderObjectsInitially();
    }

    private void UpdateFixedObjects()
    {
        foreach (Hyperobject fixedObject in fixedObjects)
        {
            foreach (ConnectedVertices connectedVertices in fixedObject.vertices)
            {
                rendering.ProjectFixedVertices(connectedVertices, fixedObject, cameraState.rotation, !hyperscene!.IsFixed || hyperscene.IsOrthographic);
            }
        }
    }
}
