using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
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
        Fixed5Cell,
        Fixed16Cell,
        FixedExtrudingCube,
        FixedRotationalPlanes
    }

    public static HypersceneRenderer instance { get; private set; } = null!;

    [SerializeField]
    public HypersceneOption hypersceneOption { get; private set; } = HypersceneOption.Default;
    public Hyperscene? hyperscene { get; private set; } = null;

    private CameraPosition cameraPosition = null!;
    private CameraRotation cameraRotation = null!;
    private CameraState cameraState = null!;
    private Rendering rendering = null!;

    private readonly float fov = Mathf.PI / 8f;

    private int lastRerenderAllFrame = -1;

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

    private void LateUpdate()
    {
        (HashSet<Hyperobject>?, HashSet<Hyperobject>?) rerenderObjects = hyperscene!.Update();

        RerenderSpecificObjects(rerenderObjects.Item1, rerenderObjects.Item2);
    }
    public void OnSceneSpecificSliderUpdate(float value)
    {
        (HashSet<Hyperobject>?, HashSet<Hyperobject>?) rerenderObjects = hyperscene!.OnSceneSliderUpdate(value);

        RerenderSpecificObjects(rerenderObjects.Item1, rerenderObjects.Item2);
    }
    private void RerenderSpecificObjects(HashSet<Hyperobject>? objects, HashSet<Hyperobject>? fixedObjects)
    {
        if (objects != null)
        {
            foreach (Hyperobject rerenderObject in objects)
            {
                _ = !rendering.RemoveSingleObject(rerenderObject);

                foreach (ConnectedVertices connectedVertices in rerenderObject.connectedVertices)
                {
                    rendering.ProjectVertices(connectedVertices, rerenderObject, cameraState.rotation, cameraState.position);
                }
            }
        }
        if (fixedObjects != null)
        {
            foreach (Hyperobject rerenderObject in fixedObjects)
            {
                _ = !rendering.RemoveSingleObject(rerenderObject);

                foreach (ConnectedVertices connectedVertices in rerenderObject.connectedVertices)
                {
                    rendering.ProjectFixedVertices(connectedVertices, rerenderObject, cameraState.rotation, !hyperscene!.IsFixed || hyperscene.IsOrthographic);
                }
            }
        }
    }

    public void LoadHyperscene(HypersceneOption hypersceneOption)
    {
        rendering.ClearAllRenderedObjects();
        hyperscene!.Objects.Clear();
        hyperscene!.FixedObjects.Clear();

        this.hypersceneOption = hypersceneOption;
        cameraState.ResetRotation();

        InitializeHyperscene();
    }

    private void InitializeHyperscene()
    {
        switch (hypersceneOption)
        {
            case HypersceneOption.Default:
                //hyperscene = new VideoHypercubesHyperscene();
                //hyperscene = new VideoMovementHyperscene();
                //hyperscene = new VideoRotationFirstpersonHyperscene();

                hyperscene = new DefaultHyperscene();
                break;
            case HypersceneOption.Ground:
                hyperscene = new GroundHyperscene();
                break;
            case HypersceneOption.FixedTesseract:
                hyperscene = new FixedTesseractHyperscene();
                break;
            case HypersceneOption.Fixed5Cell:
                hyperscene = new Fixed5CellHyperscene();
                break;
            case HypersceneOption.Fixed16Cell:
                hyperscene = new Fixed16CellHyperscene();
                break;
            case HypersceneOption.FixedExtrudingCube:
                hyperscene = new FixedExtrudingCubeHyperscene();
                break;
            case HypersceneOption.FixedRotationalPlanes:
                hyperscene = new FixedRotationalPlanesHyperscene();
                break;
            default:
                Debug.LogError("HypersceneRenderer: Unknown hyperscene option.");
                break;
        }

        hyperscene!.Start();

        if (hyperscene!.IsFixed && hyperscene!.Objects.Count > 0)
        {
            Debug.LogWarning("HypersceneRenderer: Fixed hyperscenes should not have any non-fixed objects.");
        }

        cameraPosition.enabled = !hyperscene.IsFixed;
        cameraState.sceneUiHandler.SetSceneSpecificSliderActive(hyperscene.ShowSceneSlider);
        cameraState.sceneUiHandler.SetSceneSpecificSliderValue(0f);

        RenderObjectsInitially();

        OnSceneSpecificSliderUpdate(0f);
    }

    public void RerenderAll()
    {
        if (Time.frameCount == lastRerenderAllFrame)
            return; // Prevent multiple rerenders in the same frame
        lastRerenderAllFrame = Time.frameCount;

        rendering.ClearAllRenderedObjects();

        foreach (Hyperobject obj in hyperscene!.Objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.connectedVertices)
            {
                rendering.ProjectVertices(connectedVertices, obj, cameraState.rotation, cameraState.position);
            }
        }

        UpdateFixedObjects();
    }

    public void RenderObjectsInitially()
    {
        cameraState.SetPosition(hyperscene!.StartingPosition);

        foreach (Hyperobject obj in hyperscene!.Objects)
        {
            Vector4 pos = obj.position - cameraState.position;

            foreach (ConnectedVertices connectedVertices in obj.connectedVertices)
            {
                rendering.ProjectVertices(connectedVertices, obj, cameraState.rotation, cameraState.position);
            }
        }

        // Project the fixed objects
        foreach (Hyperobject fixedObject in hyperscene!.FixedObjects)
        {
            foreach (ConnectedVertices connectedVertices in fixedObject.connectedVertices)
            {
                rendering.ProjectFixedVertices(connectedVertices, fixedObject, cameraState.rotation, !hyperscene.IsFixed || hyperscene.IsOrthographic);
            }
        }
    }
    public void RerenderObjects()
    {
        rendering.ClearAllRenderedObjects();

        foreach (Hyperobject obj in hyperscene!.Objects)
        {
            foreach (ConnectedVertices connectedVertices in obj.connectedVertices)
            {
                rendering.ProjectVertices(connectedVertices, obj, cameraState.rotation, cameraState.position);
            }
        }

        UpdateFixedObjects();
    }

    public void ReloadScene()
    {
        rendering.ClearAllRenderedObjects();
        RenderObjectsInitially();
    }

    private void UpdateFixedObjects()
    {
        foreach (Hyperobject fixedObject in hyperscene!.FixedObjects)
        {
            foreach (ConnectedVertices connectedVertices in fixedObject.connectedVertices)
            {
                rendering.ProjectFixedVertices(connectedVertices, fixedObject, cameraState.rotation, !hyperscene!.IsFixed || hyperscene.IsOrthographic);
            }
        }
    }
}
