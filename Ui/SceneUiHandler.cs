using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField]
    private Button resetRotationButton;
    [SerializeField]
    private Button applyChangesButton;

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
    private TMP_InputField lwInput;
    [SerializeField]
    private TMP_InputField lxInput;
    [SerializeField]
    private TMP_InputField lyInput;
    [SerializeField]
    private TMP_InputField lzInput;
    [SerializeField]
    private TMP_InputField rwInput;
    [SerializeField]
    private TMP_InputField rxInput;
    [SerializeField]
    private TMP_InputField ryInput;
    [SerializeField]
    private TMP_InputField rzInput;

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

        previousCameraStateEnabled = cameraState.enabled;
        previousCameraMovementEnabled = cameraState.cameraMovement.enabled;
        previousCameraRotationEnabled = cameraState.cameraRotation.enabled;

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
            lwInput.SetTextWithoutNotify(rotationQuat.leftQuaternion .w.ToString("F6"));
            lxInput.SetTextWithoutNotify(rotationQuat.leftQuaternion .x.ToString("F6"));
            lyInput.SetTextWithoutNotify(rotationQuat.leftQuaternion .y.ToString("F6"));
            lzInput.SetTextWithoutNotify(rotationQuat.leftQuaternion .z.ToString("F6"));
            rwInput.SetTextWithoutNotify(rotationQuat.rightQuaternion.w.ToString("F6"));
            rxInput.SetTextWithoutNotify(rotationQuat.rightQuaternion.x.ToString("F6"));
            ryInput.SetTextWithoutNotify(rotationQuat.rightQuaternion.y.ToString("F6"));
            rzInput.SetTextWithoutNotify(rotationQuat.rightQuaternion.z.ToString("F6"));
        }
    }

    private bool isEditingText = false;
    private Rotation4 newRotation = Rotation4.identity;
    private bool previousCameraMovementEnabled;
    private bool previousCameraRotationEnabled;
    private bool previousCameraStateEnabled;
    public void DisableGlobalInput()
    {
        if (isEditingText)
            return;

        isEditingText = true;

        previousCameraMovementEnabled = cameraState.cameraMovement.enabled;
        previousCameraRotationEnabled = cameraState.cameraRotation.enabled;
        previousCameraStateEnabled = cameraState.enabled;
        cameraState.cameraMovement.enabled = false;
        cameraState.cameraRotation.enabled = false;
        cameraState.enabled = false;

        newRotation = cameraState.rotation;

        // TODO: Tab navigation if in input field
    }
    public void ReenableGlobalInput()
    {
        cameraState.enabled = previousCameraStateEnabled;
        cameraState.cameraMovement.enabled = previousCameraMovementEnabled;
        cameraState.cameraRotation.enabled = previousCameraRotationEnabled;

        resetRotationButton.gameObject.SetActive(true);
        applyChangesButton.gameObject.SetActive(false);

        isEditingText = false;
    }
    public void OnInputChanged()
    {
        if (!isEditingText)
            DisableGlobalInput();

        resetRotationButton.gameObject.SetActive(false);
        applyChangesButton.gameObject.SetActive(true);

        if (!float.TryParse(lwInput.text, out float lw))
            lw = 0f;
        if (!float.TryParse(lxInput.text, out float lx))
            lx = 0f;
        if (!float.TryParse(lyInput.text, out float ly))
            ly = 0f;
        if (!float.TryParse(lzInput.text, out float lz))
            lz = 0f;
        if (!float.TryParse(rwInput.text, out float rw))
            rw = 0f;
        if (!float.TryParse(rxInput.text, out float rx))
            rx = 0f;
        if (!float.TryParse(ryInput.text, out float ry))
            ry = 0f;
        if (!float.TryParse(rzInput.text, out float rz))
            rz = 0f;

        newRotation = new Rotation4(
            new Quaternion(lx, ly, lz, lw),
            new Quaternion(rx, ry, rz, rw)
        );
    }
    public void OnApplyChangesButtonClicked()
    {
        cameraState.UpdateRotation(newRotation);
        ReenableGlobalInput();
    }

    public void OnRotationSliderChange()
    {
        RotationEuler4 sliderRotationEuler = new RotationEuler4(xwSlider.value, ywSlider.value, zwSlider.value, xySlider.value, xzSlider.value, yzSlider.value);
        RotationEuler4 rotationDeltaEuler = sliderRotationEuler - cameraState.rotationEuler;

        cameraState.UpdateRotationDelta(rotationDeltaEuler);
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

        if (!displayQuaternionPair)
        {
            ReenableGlobalInput();
        }
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
