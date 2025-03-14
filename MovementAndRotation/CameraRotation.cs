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
    private static KeyCode RotateXWPos = USE_DVORAK ? KeyCode.N : KeyCode.L;
    private static KeyCode RotateXWNeg = USE_DVORAK ? KeyCode.H : KeyCode.J;

    private static KeyCode RotateYWPos = USE_DVORAK ? KeyCode.R : KeyCode.O;
    private static KeyCode RotateYWNeg = USE_DVORAK ? KeyCode.G : KeyCode.U;

    private static KeyCode RotateZWPos = USE_DVORAK ? KeyCode.C : KeyCode.I;
    private static KeyCode RotateZWNeg = USE_DVORAK ? KeyCode.T : KeyCode.K;

    private void Awake()
    {
        cameraPos = GetComponent<CameraPosition>();
    }

    private void Update()
    {
        float speed = rotationSpeed * Time.deltaTime;
        bool rotationUpdated = false;

        if (Input.GetKey(RotateXWPos))
        {
            cameraPos.rotation.xw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(RotateXWNeg))
        {
            cameraPos.rotation.xw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(RotateYWPos))
        {
            cameraPos.rotation.yw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(RotateYWNeg))
        {
            cameraPos.rotation.yw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(RotateZWPos))
        {
            cameraPos.rotation.zw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(RotateZWNeg))
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