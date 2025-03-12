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

    private void Awake()
    {
        cameraPos = GetComponent<CameraPosition>();
    }

    public void OnSliderChange()
    {
        cameraPos.rotation = new Rotation4(xySlider.value, xzSlider.value, yzSlider.value, xwSlider.value, ywSlider.value, zwSlider.value);

        onRotationUpdate();
    }
}