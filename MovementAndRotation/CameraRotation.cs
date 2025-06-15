using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CameraState))]
public class CameraRotation : MonoBehaviour
{
    private CameraState cameraState;

    [SerializeField]
    private Slider xwSlider;
    [SerializeField]
    private Slider ywSlider;
    [SerializeField]
    private Slider zwSlider;
    [SerializeField]
    private Slider xySlider;
    [SerializeField]
    private Slider xzSlider;
    [SerializeField]
    private Slider yzSlider;

    [SerializeField]
    private float rotationSpeed;

    [HideInInspector]
    public KeyCode RotateXWPos;
    [HideInInspector]
    public KeyCode RotateXWNeg;
    [HideInInspector]
    public KeyCode RotateYWPos;
    [HideInInspector]
    public KeyCode RotateYWNeg;
    [HideInInspector]
    public KeyCode RotateZWPos;
    [HideInInspector]
    public KeyCode RotateZWNeg;

    private void Awake()
    {
        cameraState = GetComponent<CameraState>();

        RotateXWPos = cameraState.useDvorak ? KeyCode.N : KeyCode.L;
        RotateXWNeg = cameraState.useDvorak ? KeyCode.H : KeyCode.J;
        RotateYWPos = cameraState.useDvorak ? KeyCode.R : KeyCode.O;
        RotateYWNeg = cameraState.useDvorak ? KeyCode.G : KeyCode.U;
        RotateZWPos = cameraState.useDvorak ? KeyCode.C : KeyCode.I;
        RotateZWNeg = cameraState.useDvorak ? KeyCode.T : KeyCode.K;
    }

    private void Update()
    {
        float speed = rotationSpeed * Time.deltaTime;
        bool rotationUpdated = false;
        Rotation4 rotationDelta = new Rotation4(0, 0, 0, 0, 0, 0);

        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateXWPos : cameraState.cameraMovement.MoveRight))
        {
            rotationDelta.xw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateXWNeg : cameraState.cameraMovement.MoveLeft))
        {
            rotationDelta.xw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateYWPos : cameraState.cameraMovement.MoveUp))
        {
            rotationDelta.yw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateYWNeg : cameraState.cameraMovement.MoveDown))
        {
            rotationDelta.yw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateZWPos : cameraState.cameraMovement.MoveForwards))
        {
            rotationDelta.zw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateZWNeg : cameraState.cameraMovement.MoveBackwards))
        {
            rotationDelta.zw -= speed;
            rotationUpdated = true;
        }

        if (rotationUpdated)
        {
            rotationDelta.ModuloPlanes();
            UpdateSliderValues();
            cameraState.UpdateRotation(rotationDelta);
        }
    }

    private void UpdateSliderValues()
    {
        xwSlider.SetValueWithoutNotify(cameraState.rotation.xw);
        ywSlider.SetValueWithoutNotify(cameraState.rotation.yw);
        zwSlider.SetValueWithoutNotify(cameraState.rotation.zw);
        xySlider.SetValueWithoutNotify(cameraState.rotation.xy);
        xzSlider.SetValueWithoutNotify(cameraState.rotation.xz);
        yzSlider.SetValueWithoutNotify(cameraState.rotation.yz);
    }

    public void OnSliderChange()
    {
        Rotation4 sliderRotation = new Rotation4(xwSlider.value, ywSlider.value, zwSlider.value, xySlider.value, xzSlider.value, yzSlider.value);
        Rotation4 rotationDelta = sliderRotation - cameraState.rotation;

        cameraState.UpdateRotation(rotationDelta);
    }


    // Entry-point for reset button in the UI
    public void ResetRotation()
    {
        HypersceneRenderer hypersceneRenderer = GetComponent<HypersceneRenderer>();

        cameraState.ResetRotationValues();

        hypersceneRenderer.ResetRotation();

        UpdateSliderValues();
    }
}