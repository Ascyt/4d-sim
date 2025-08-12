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
    public static CameraState instance { get; private set; }

    /// <summary>
    /// Use the dvorak keyboard layout for movement and rotation controls.
    /// </summary>
    public bool useDvorak = true;

    // TODO: This should make part of the rotation relative to world space, not the camera's forward direction.
    // In 3D, while the YZ (up/down) rotation and the XY camera tilt are relative to the camera's forward direction, the XZ (left/right) rotation should be relative to the world space.
    // For 4D, I could do a similar thing, make YW and camera tilt relative to the camera's forward direction and XW relative to world space,
    // but the issue is that I'm not sure what to do with ZW. I'm also not sure if it should be uncapped like the XZ rotation in 3D, or also stopped like when looking straight up or down in 3D?
    public bool platformerMode; 

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
    public Quatpair rotation = new Quatpair();

    ///// <summary>
    ///// Used to display the rotation in the UI, and to update the rotation slider values.
    ///// </summary>
    //public RotationEuler4 rotationEuler = RotationEuler4.zero; 

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
        sceneUiHandler.UpdateQuaternionPairRotationSliders(rotation);
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
    public void SetRotation(RotationEuler4 rotation)
    {
        this.rotation = new Quatpair(rotation);

        sceneUiHandler.UpdateQuaternionPairRotationSliders(this.rotation);
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
        Vector4 rotatedPositionDelta = positionDelta.ApplyRotation(rotation);
        position += rotatedPositionDelta;

        hypersceneRenderer.RerenderAll();

        sceneUiHandler.UpdatePositionText(position);
    }
    public void UpdateRotationDelta(RotationEuler4 rotationDelta)
    {
        if (platformerMode)
        {
            RotationEuler4 absolutePart = new RotationEuler4(
                rotationDelta.xw,
                0,
                rotationDelta.zw,
                0,
                0,
                0);
            RotationEuler4 relativePart = new RotationEuler4(
                0,
                rotationDelta.yw,
                0,
                rotationDelta.xy,
                rotationDelta.xz,
                rotationDelta.yz);

            rotation = rotation.ApplyRotation(absolutePart, true);
            rotation = rotation.ApplyRotation(relativePart, false);
        }
        else
        {
            rotation = rotation.ApplyRotation(rotationDelta, false);
        }

        hypersceneRenderer.RerenderAll();

        sceneUiHandler.UpdateQuaternionPairRotationSliders(rotation);
    }
    public void UpdateRotation(Quatpair newRotation)
    {
        rotation = newRotation;

        hypersceneRenderer.RerenderAll();

        sceneUiHandler.UpdateQuaternionPairRotationSliders(rotation);
    }
}
