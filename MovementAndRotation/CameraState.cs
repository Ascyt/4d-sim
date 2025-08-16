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

    public bool absoluteMode; 
    public Vector3 absoluteModeRotationAngles = Vector3.zero;

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
        rotation = Quatpair.identity;
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
        newValue ??= !RotationMovementSwitch;
        RotationMovementSwitch = newValue.Value;

        sceneUiHandler.UpdateMovementRotationSwitchText(RotationMovementSwitch);
    }

    /// <summary>
    /// Does not cause view refresh
    /// </summary>
    public void ResetRotation()
    {
        UpdateRotation(Quatpair.identity);
        absoluteModeRotationAngles = Vector3.zero;

        sceneUiHandler.UpdateQuaternionPairRotationSliders(rotation);
        sceneUiHandler.OnAbsoluteRotationChange(Vector3.zero);
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
        if (absoluteMode)
        {
            absoluteModeRotationAngles += new Vector3(rotationDelta.xw, rotationDelta.yw, rotationDelta.zw);

            float epsilon = 1f / 4096f;

            absoluteModeRotationAngles.y = Mathf.Clamp(absoluteModeRotationAngles.y, -Mathf.PI / 2f + epsilon, Mathf.PI / 2f - epsilon);
            absoluteModeRotationAngles.z = Mathf.Clamp(absoluteModeRotationAngles.z, -Mathf.PI / 2f + epsilon, Mathf.PI / 2f - epsilon);
            
            rotation = Quatpair.identity
                .ApplyRotationInSinglePlane(RotationTransformer.RotationPlane.XW, absoluteModeRotationAngles.x, false)
                .ApplyRotationInSinglePlane(RotationTransformer.RotationPlane.YW, absoluteModeRotationAngles.y, false)
                .ApplyRotationInSinglePlane(RotationTransformer.RotationPlane.ZW, absoluteModeRotationAngles.z, false);

            sceneUiHandler.OnAbsoluteRotationChange(absoluteModeRotationAngles);
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
