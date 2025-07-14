using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// I/O points for the scene UI.
/// </summary>
public class SceneUiHandler : MonoBehaviour
{
    public static SceneUiHandler instance { get; private set; }
    [HideInInspector]
    public CameraState cameraState;

    [SerializeField]
    private TextMeshProUGUI positionText;
    [SerializeField]
    private TextMeshProUGUI rotationText;

    [Space] 

    [SerializeField]
    private TextMeshProUGUI fpsText;
    [SerializeField]
    private TextMeshProUGUI movementRotationSwitchText;

    [Space]
    [SerializeField]
    private bool displayQuaternionPair = false;
    [SerializeField]
    private TextMeshProUGUI switchButtonText;
    [SerializeField]
    private GameObject eulerAnglesParent;
    [SerializeField]
    private GameObject quaternionPairParent;

    [Space]
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

    [Space]
    [SerializeField]
    private Slider lwSlider;
    [SerializeField]
    private Slider lxSlider;
    [SerializeField]
    private Slider lySlider;
    [SerializeField]
    private Slider lzSlider;
    [SerializeField]
    private Slider rwSlider;
    [SerializeField]
    private Slider rxSlider;
    [SerializeField]
    private Slider rySlider;
    [SerializeField]
    private Slider rzSlider;

    [Space]
    [SerializeField]
    private TMP_Dropdown hypersceneDropdown;

    private float lastAvgFpsUpdate = 0f;
    private int frameCount = 0;
    private const float FPS_UPDATE_INTERVAL = 1f / 4f;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        hypersceneDropdown.AddOptions(Enum
            .GetNames(typeof(HypersceneRenderer.HypersceneOption))
            .Select(option => new TMP_Dropdown.OptionData(option))
            .ToList());

        hypersceneDropdown.SetValueWithoutNotify((int)cameraState.hypersceneRenderer.hypersceneOption);

        UpdateDisplay();
    }
    private void Update()
    {
        UpdateAvgFps();
    }

    private void UpdateAvgFps()
    {
        frameCount++;

        if (Time.unscaledTime - lastAvgFpsUpdate < FPS_UPDATE_INTERVAL)
            return;

        float avgFps = frameCount / (Time.unscaledTime - lastAvgFpsUpdate) / FPS_UPDATE_INTERVAL;

        lastAvgFpsUpdate = Time.unscaledTime;
        frameCount = 0;

        fpsText.text = $"Avg FPS: {avgFps:F2}";
    }

    public void UpdateMovementRotationSwitchText(bool newSwitch)
    {
        movementRotationSwitchText.text = $"Rotation/Movement switched: {newSwitch}";
    }

    public void UpdatePositionText(Vector4 position)
    {
        positionText.text =
        $"x: {position.x}\n" +
        $"y: {position.y}\n" +
        $"z: {position.z}\n" +
        $"w: {position.w}";
    }

    public void UpdateRotationText(RotationEuler4 rotationEuler, Rotation4 rotationQuat)
    {
        if (!displayQuaternionPair)
        {
            rotationText.text =
                $"xw: {rotationEuler.xw * Mathf.Rad2Deg}°\n" +
                $"yw: {rotationEuler.yw * Mathf.Rad2Deg}°\n" +
                $"zw: {rotationEuler.zw * Mathf.Rad2Deg}°\n" +
                $"xy: {rotationEuler.xy * Mathf.Rad2Deg}°\n" +
                $"xz: {rotationEuler.xz * Mathf.Rad2Deg}°\n" +
                $"yz: {rotationEuler.yz * Mathf.Rad2Deg}°";
        }
        else
        {
            rotationText.text =
                $"L.w: {rotationQuat.leftQuaternion.w}\n" +
                $"L.x: {rotationQuat.leftQuaternion.x}\n" +
                $"L.y: {rotationQuat.leftQuaternion.y}\n" +
                $"L.z: {rotationQuat.leftQuaternion.z}\n" +
                $"R.w: {rotationQuat.rightQuaternion.w}\n" +
                $"R.x: {rotationQuat.rightQuaternion.x}\n" +
                $"R.y: {rotationQuat.rightQuaternion.y}\n" +
                $"R.z: {rotationQuat.rightQuaternion.z}\n";
        }
    }

    public void UpdateRotationSliderValues(RotationEuler4 rotationEuler, Rotation4 rotationQuat)
    {
        if (!displayQuaternionPair)
        {
            xwSlider.SetValueWithoutNotify(rotationEuler.xw);
            ywSlider.SetValueWithoutNotify(rotationEuler.yw);
            zwSlider.SetValueWithoutNotify(rotationEuler.zw);
            xySlider.SetValueWithoutNotify(rotationEuler.xy);
            xzSlider.SetValueWithoutNotify(rotationEuler.xz);
            yzSlider.SetValueWithoutNotify(rotationEuler.yz);
        }
        else
        {
            lwSlider.SetValueWithoutNotify(rotationQuat.leftQuaternion.w);
            lxSlider.SetValueWithoutNotify(rotationQuat.leftQuaternion.x);
            lySlider.SetValueWithoutNotify(rotationQuat.leftQuaternion.y);
            lzSlider.SetValueWithoutNotify(rotationQuat.leftQuaternion.z);
            rwSlider.SetValueWithoutNotify(rotationQuat.rightQuaternion.w);
            rxSlider.SetValueWithoutNotify(rotationQuat.rightQuaternion.x);
            rySlider.SetValueWithoutNotify(rotationQuat.rightQuaternion.y);
            rzSlider.SetValueWithoutNotify(rotationQuat.rightQuaternion.z);
        }
    }

    public void OnRotationSliderChange()
    {
        if (!displayQuaternionPair)
        {
            RotationEuler4 sliderRotationEuler = new RotationEuler4(xwSlider.value, ywSlider.value, zwSlider.value, xySlider.value, xzSlider.value, yzSlider.value);
            RotationEuler4 rotationDeltaEuler = sliderRotationEuler - cameraState.rotationEuler;

            cameraState.UpdateRotationDelta(rotationDeltaEuler);
        }
        else
        {
            Rotation4 sliderRotationQuat = new Rotation4(
                new Quaternion(lxSlider.value, lySlider.value, lzSlider.value, lwSlider.value),
                new Quaternion(rxSlider.value, rySlider.value, rzSlider.value, rwSlider.value)
            );

            cameraState.UpdateRotation(sliderRotationQuat);
        }
    }

    public void SwitchDisplay()
    {
        displayQuaternionPair = !displayQuaternionPair;

        UpdateDisplay();
    }
    private void UpdateDisplay()
    {
        eulerAnglesParent.SetActive(!displayQuaternionPair);
        quaternionPairParent.SetActive(displayQuaternionPair);

        switchButtonText.text = displayQuaternionPair ? "Quaternion Pair <=>" : "Euler Angles <=>";

        UpdateRotationText(cameraState.rotationEuler, cameraState.rotation);
        UpdateRotationSliderValues(cameraState.rotationEuler, cameraState.rotation);
    }

    public void ResetRotation()
    {
        cameraState.SetRotation(RotationEuler4.zero);

        cameraState.hypersceneRenderer.ResetRotation();
    }

    public void OnDropdownValueChanged(int id)
    {
        int value = hypersceneDropdown.value;

        HypersceneRenderer.HypersceneOption selectedOption = (HypersceneRenderer.HypersceneOption)value;
        cameraState.hypersceneRenderer.LoadHyperscene(selectedOption);
    }
}
