using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CameraPosition))]
[RequireComponent(typeof(CameraRotation))]
public class CameraState : MonoBehaviour
{
    public CameraState instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI positionText;
    [SerializeField]
    private TextMeshProUGUI rotationText;

    public bool useDvorak = true;
    // TODO: This should make part of the rotation relative to world space, not the camera's forward direction.
    // In 3D, while the YZ rotation and the XY camera tilt are relative to the camera's forward direction, the XZ rotation should be relative to the world space.
    // For 4D, I could do a similar thing, make YW and camera tilt relative to the camera's forward direction and XW relative to world space,
    // but the issue is that I'm not sure what to do with ZW. And should it be uncapped like the XZ rotation in 3D, or also stopped like when looking straight up or down in 3D?
    // public bool platformerMode; 

    [HideInInspector]
    public CameraPosition cameraMovement;
    [HideInInspector]
    public CameraRotation cameraRotation;

    public bool RotationMovementSwitch { get; private set; } = false;

    private readonly KeyCode movementRotationSwitchKey = KeyCode.Tab;

    [SerializeField]
    private TextMeshProUGUI movementRotationSwitchText;

    public Vector4 position = new Vector4(0, 0, 0, 0);
    public Rotation4 rotation = new Rotation4(0, 0, 0, 0, 0, 0);

    private Vector4 right = new Vector4(1, 0, 0, 0);
    private Vector4 up = new Vector4(0, 1, 0, 0);
    private Vector4 forward = new Vector4(0, 0, 1, 0);
    private Vector4 ana = new Vector4(0, 0, 0, 1);

    private void Awake()
    {
        instance = this;

        cameraMovement = GetComponent<CameraPosition>();
        cameraRotation = GetComponent<CameraRotation>();

        cameraMovement.onPositionUpdate += OnPositionUpdate;
        cameraRotation.onRotationUpdate += OnRotationUpdate;
    }
    private void Start()
    {
        UpdateMovementRotationSwitch(RotationMovementSwitch);
        OnValuesUpdateSelf();
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
        movementRotationSwitchText.text = $"Rotation/Movement switched: {RotationMovementSwitch}";
    }

    public void ResetRotationValues()
    {
        rotation = Rotation4.zero;

        right = new Vector4(1, 0, 0, 0);
        up = new Vector4(0, 1, 0, 0);
        forward = new Vector4(0, 0, 1, 0);
        ana = new Vector4(0, 0, 0, 1);

        OnValuesUpdateSelf();
    }

    private void OnPositionUpdate(Vector4 positionDelta)
    {
        position += (positionDelta.x * right) + (positionDelta.y * up) + (positionDelta.z * forward) + (positionDelta.w * ana);
        OnValuesUpdateSelf();
    }
    private void OnRotationUpdate(Rotation4 rotationDelta)
    {
        rotation += rotationDelta;

        right = right.Rotate(rotationDelta);
        up = up.Rotate(rotationDelta);
        forward = forward.Rotate(rotationDelta);
        ana = ana.Rotate(rotationDelta);

        OnValuesUpdateSelf();
    }

    private void OnValuesUpdateSelf()
    {
        positionText.text = 
            $"x: {position.x}\n" +
            $"y: {position.y}\n" +
            $"z: {position.z}\n" +
            $"w: {position.w}";
        rotationText.text =
            $"xw: {rotation.xw * Mathf.Rad2Deg}°\n" +
            $"yw: {rotation.yw * Mathf.Rad2Deg}°\n" +
            $"zw: {rotation.zw * Mathf.Rad2Deg}°\n" +
            $"xy: {rotation.xy * Mathf.Rad2Deg}°\n" +
            $"xz: {rotation.xz * Mathf.Rad2Deg}°\n" +
            $"yz: {rotation.yz * Mathf.Rad2Deg}°";
    }
}
