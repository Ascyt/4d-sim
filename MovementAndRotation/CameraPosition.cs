using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public CameraPosition instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI positionText;
    [SerializeField]
    private TextMeshProUGUI rotationText;

    [SerializeField]
    public bool platformerMode;

    private CameraMovement cameraMovement;
    private CameraRotation cameraRotation;

    [SerializeField]
    public bool movementRotationSwitch = false;
    private KeyCode movementRotationSwitchKey = KeyCode.Tab;

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

        cameraMovement = GetComponent<CameraMovement>();
        cameraRotation = GetComponent<CameraRotation>();

        cameraMovement.onPositionUpdate += OnPositionUpdate;
        cameraRotation.onRotationUpdate += OnRotationUpdate;
    }
    private void Start()
    {
        UpdateMovementRotationSwitch(movementRotationSwitch);
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
        newValue = newValue ?? !movementRotationSwitch;
        movementRotationSwitch = newValue.Value;
        movementRotationSwitchText.text = $"Movement/Rotation switched: {movementRotationSwitch}";
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
