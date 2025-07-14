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

    public void UpdateRotationText(RotationEuler4 rotation)
    {
        rotationText.text =
            $"xw: {rotation.xw * Mathf.Rad2Deg}°\n" +
            $"yw: {rotation.yw * Mathf.Rad2Deg}°\n" +
            $"zw: {rotation.zw * Mathf.Rad2Deg}°\n" +
            $"xy: {rotation.xy * Mathf.Rad2Deg}°\n" +
            $"xz: {rotation.xz * Mathf.Rad2Deg}°\n" +
            $"yz: {rotation.yz * Mathf.Rad2Deg}°";
    }

    public void UpdateRotationSliderValues(RotationEuler4 rotation)
    {
        xwSlider.SetValueWithoutNotify(rotation.xw);
        ywSlider.SetValueWithoutNotify(rotation.yw);
        zwSlider.SetValueWithoutNotify(rotation.zw);
        xySlider.SetValueWithoutNotify(rotation.xy);
        xzSlider.SetValueWithoutNotify(rotation.xz);
        yzSlider.SetValueWithoutNotify(rotation.yz);
    }

    public void OnRotationSliderChange()
    {
        RotationEuler4 sliderRotation = new RotationEuler4(xwSlider.value, ywSlider.value, zwSlider.value, xySlider.value, xzSlider.value, yzSlider.value);
        RotationEuler4 rotationDelta = sliderRotation - cameraState.rotationEuler;

        cameraState.UpdateRotation(rotationDelta);
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
