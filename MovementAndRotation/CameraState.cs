using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the camera state, including position and rotation. 
/// </summary>
[RequireComponent(typeof(CameraPosition))]
[RequireComponent(typeof(CameraRotation))]
[RequireComponent(typeof(HypersceneRenderer))]
public class CameraState : MonoBehaviour
{
    public CameraState instance { get; private set; }

    /// <summary>
    /// Use the dvorak keyboard layout for movement and rotation controls.
    /// </summary>
    public bool useDvorak = true;

    // TODO: This should make part of the rotation relative to world space, not the camera's forward direction.
    // In 3D, while the YZ (up/down) rotation and the XY camera tilt are relative to the camera's forward direction, the XZ (left/right) rotation should be relative to the world space.
    // For 4D, I could do a similar thing, make YW and camera tilt relative to the camera's forward direction and XW relative to world space,
    // but the issue is that I'm not sure what to do with ZW. I'm also not sure if it should be uncapped like the XZ rotation in 3D, or also stopped like when looking straight up or down in 3D?
    //public bool platformerMode; 

    [HideInInspector]
    public CameraPosition cameraMovement;
    [HideInInspector]
    public CameraRotation cameraRotation;
    [HideInInspector]
    public HypersceneRenderer hypersceneRenderer;

    [SerializeField]
    private SceneUiHandler sceneUiHandler;

    public bool RotationMovementSwitch { get; private set; } = false;

    private readonly KeyCode movementRotationSwitchKey = KeyCode.Tab;

    public Vector4 position = Vector4.zero;
    public Rotation4 rotation = Rotation4.zero;

    // These vectors rotate with the camera and are used for movement calculations.
    private Vector4 right = new Vector4(1, 0, 0, 0);
    private Vector4 up = new Vector4(0, 1, 0, 0);
    private Vector4 forward = new Vector4(0, 0, 1, 0);
    private Vector4 ana = new Vector4(0, 0, 0, 1);

    private void Awake()
    {
        instance = this;

        cameraMovement = GetComponent<CameraPosition>();
        cameraRotation = GetComponent<CameraRotation>();
        hypersceneRenderer = GetComponent<HypersceneRenderer>();

        sceneUiHandler.cameraState = this;
    }
    private void Start()
    {
        UpdateMovementRotationSwitch(RotationMovementSwitch);
        sceneUiHandler.UpdatePositionText(position);
        sceneUiHandler.UpdateRotationText(rotation);
    }
    private void Update()
    {
        if (Input.GetKeyDown(movementRotationSwitchKey))
        {
            UpdateMovementRotationSwitch();
        }
    }

    public void UpdateMovementRotationSwitch(bool? newValue = null)
    {
        newValue = newValue ?? !RotationMovementSwitch;
        RotationMovementSwitch = newValue.Value;

        sceneUiHandler.UpdateMovementRotationSwitchText(RotationMovementSwitch);
    }

    /// <summary>
    /// Does not cause view refresh
    /// </summary>
    public void SetRotation(Rotation4 rotation)
    {
        this.rotation = rotation;

        right = new Vector4(1, 0, 0, 0).Rotate(rotation);
        up = new Vector4(0, 1, 0, 0).Rotate(rotation);
        forward = new Vector4(0, 0, 1, 0).Rotate(rotation);
        ana = new Vector4(0, 0, 0, 1).Rotate(rotation);

        sceneUiHandler.UpdateRotationText(rotation);
        sceneUiHandler.UpdateRotationSliderValues(rotation);
    }
    /// <summary>
    /// Does not cause view refresh
    /// </summary>
    public void SetPosition(Vector4 position)
    {
        this.position = position;
        sceneUiHandler.UpdatePositionText(position);
    }

    public void UpdatePosition(Vector4 positionDelta)
    {
        position += (positionDelta.x * right) + (positionDelta.y * up) + (positionDelta.z * forward) + (positionDelta.w * ana);

        hypersceneRenderer.RenderObjectsCameraPositionChange(positionDelta);

        sceneUiHandler.UpdatePositionText(position);
    }
    public void UpdateRotation(Rotation4 rotationDelta)
    {
        rotation += rotationDelta;

        right = right.Rotate(rotationDelta);
        up = up.Rotate(rotationDelta);
        forward = forward.Rotate(rotationDelta);
        ana = ana.Rotate(rotationDelta);

        hypersceneRenderer.RenderObjectsCameraRotationChange(rotationDelta);

        sceneUiHandler.UpdateRotationText(rotation);
        sceneUiHandler.UpdateRotationSliderValues(rotation);
    }
}
