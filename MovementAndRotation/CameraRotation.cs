using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRotation : MonoBehaviour
{
    private CameraPosition cameraPos;

    public delegate void OnRotationUpdate();
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

    private const bool USE_DVORAK = true;
    public static readonly KeyCode RotateXWPos = USE_DVORAK ? KeyCode.N : KeyCode.L;
    public static readonly KeyCode RotateXWNeg = USE_DVORAK ? KeyCode.H : KeyCode.J;
    public static readonly KeyCode RotateYWPos = USE_DVORAK ? KeyCode.R : KeyCode.O;
    public static readonly KeyCode RotateYWNeg = USE_DVORAK ? KeyCode.G : KeyCode.U;
    public static readonly KeyCode RotateZWPos = USE_DVORAK ? KeyCode.C : KeyCode.I;
    public static readonly KeyCode RotateZWNeg = USE_DVORAK ? KeyCode.T : KeyCode.K;

    private void Awake()
    {
        cameraPos = GetComponent<CameraPosition>();
    }

    private void Update()
    {
        float speed = rotationSpeed * Time.deltaTime;
        bool rotationUpdated = false;

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? RotateXWPos : CameraMovement.MoveRight))
        {
            cameraPos.rotation.xw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? RotateXWNeg : CameraMovement.MoveLeft))
        {
            cameraPos.rotation.xw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? RotateYWPos : CameraMovement.MoveUp))
        {
            cameraPos.rotation.yw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? RotateYWNeg : CameraMovement.MoveDown))
        {
            cameraPos.rotation.yw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? RotateZWPos : CameraMovement.MoveForwards))
        {
            cameraPos.rotation.zw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? RotateZWNeg : CameraMovement.MoveBackwards))
        {
            cameraPos.rotation.zw -= speed;
            rotationUpdated = true;
        }

        if (rotationUpdated)
        {
            cameraPos.rotation.ModuloPlanes();
            UpdateSliderValues();
            onRotationUpdate();
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
        cameraPos.rotation = new Rotation4(xwSlider.value, ywSlider.value, zwSlider.value, xySlider.value, xzSlider.value, yzSlider.value);

        onRotationUpdate();
    }
}