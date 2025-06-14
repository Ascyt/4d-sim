using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CameraPosition))]
public class CameraRotation : MonoBehaviour
{
    private CameraPosition cameraPos;

    public delegate void OnRotationUpdate(Rotation4 rotationDelta);
    public OnRotationUpdate onRotationUpdate;

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
        cameraPos = GetComponent<CameraPosition>();

        RotateXWPos = cameraPos.useDvorak ? KeyCode.N : KeyCode.L;
        RotateXWNeg = cameraPos.useDvorak ? KeyCode.H : KeyCode.J;
        RotateYWPos = cameraPos.useDvorak ? KeyCode.R : KeyCode.O;
        RotateYWNeg = cameraPos.useDvorak ? KeyCode.G : KeyCode.U;
        RotateZWPos = cameraPos.useDvorak ? KeyCode.C : KeyCode.I;
        RotateZWNeg = cameraPos.useDvorak ? KeyCode.T : KeyCode.K;
    }

    private void Update()
    {
        float speed = rotationSpeed * Time.deltaTime;
        bool rotationUpdated = false;
        Rotation4 rotationDelta = new Rotation4(0, 0, 0, 0, 0, 0);

        if (Input.GetKey(cameraPos.RotationMovementSwitch ? RotateXWPos : cameraPos.cameraMovement.MoveRight))
        {
            rotationDelta.xw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraPos.RotationMovementSwitch ? RotateXWNeg : cameraPos.cameraMovement.MoveLeft))
        {
            rotationDelta.xw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(cameraPos.RotationMovementSwitch ? RotateYWPos : cameraPos.cameraMovement.MoveUp))
        {
            rotationDelta.yw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraPos.RotationMovementSwitch ? RotateYWNeg : cameraPos.cameraMovement.MoveDown))
        {
            rotationDelta.yw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(cameraPos.RotationMovementSwitch ? RotateZWPos : cameraPos.cameraMovement.MoveForwards))
        {
            rotationDelta.zw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraPos.RotationMovementSwitch ? RotateZWNeg : cameraPos.cameraMovement.MoveBackwards))
        {
            rotationDelta.zw -= speed;
            rotationUpdated = true;
        }

        if (rotationUpdated)
        {
            rotationDelta.ModuloPlanes();
            UpdateSliderValues();
            onRotationUpdate(rotationDelta);
        }
    }

    private void UpdateSliderValues()
    {
        xwSlider.SetValueWithoutNotify(cameraPos.rotation.xw);
        ywSlider.SetValueWithoutNotify(cameraPos.rotation.yw);
        zwSlider.SetValueWithoutNotify(cameraPos.rotation.zw);
        xySlider.SetValueWithoutNotify(cameraPos.rotation.xy);
        xzSlider.SetValueWithoutNotify(cameraPos.rotation.xz);
        yzSlider.SetValueWithoutNotify(cameraPos.rotation.yz);
    }

    public void OnSliderChange()
    {
        Rotation4 sliderRotation = new Rotation4(xwSlider.value, ywSlider.value, zwSlider.value, xySlider.value, xzSlider.value, yzSlider.value);
        Rotation4 rotationDelta = sliderRotation - cameraPos.rotation;

        onRotationUpdate(rotationDelta);
    }


    // Entry-point for reset button in the UI
    public void ResetRotation()
    {
        HypersceneRenderer hypersceneRenderer = GetComponent<HypersceneRenderer>();

        cameraPos.ResetRotationValues();

        hypersceneRenderer.ResetRotation();

        UpdateSliderValues();
    }
}