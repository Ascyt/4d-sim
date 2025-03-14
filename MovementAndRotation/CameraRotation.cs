using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRotation : MonoBehaviour
{
    private CameraPosition cameraPos;

    public delegate void OnRotationUpdate();
    public OnRotationUpdate onRotationUpdate;

    public Slider xySlider;
    public Slider xzSlider;
    public Slider yzSlider;
    public Slider xwSlider;
    public Slider ywSlider;
    public Slider zwSlider;

    public float rotationSpeed;

    private void Awake()
    {
        cameraPos = GetComponent<CameraPosition>();
    }

    private void Update()
    {
        
    }

    public void UpdateSliderValues()
    {
        xwSlider.value = cameraPos.rotation.xw;
        ywSlider.value = cameraPos.rotation.yw;
        zwSlider.value = cameraPos.rotation.zw;
        xySlider.value = cameraPos.rotation.xy;
        xzSlider.value = cameraPos.rotation.xz;
        yzSlider.value = cameraPos.rotation.yz;
    }

    public void OnSliderChange()
    {
        cameraPos.rotation = new Rotation4(xwSlider.value, ywSlider.value, zwSlider.value, xySlider.value, xzSlider.value, yzSlider.value);

        onRotationUpdate();
    }
}