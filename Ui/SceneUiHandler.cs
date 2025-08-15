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
    private bool lockSliders = false;
    [SerializeField]
    private Image lockSlidersImage;
    [SerializeField]
    private Sprite lockSliderEnabledSprite;
    [SerializeField]
    private Sprite lockSliderDisabledSprite;

    [Space]
    [SerializeField]
    private bool absoluteMode = false;
    [SerializeField]
    private Image absoluteModeImage;
    [SerializeField]
    private Sprite absoluteModeEnabledSprite;
    [SerializeField]
    private Sprite absoluteModeDisabledSprite;
    [SerializeField]
    private Slider xwSliderAbsolute;
    [SerializeField]
    private Slider ywSliderAbsolute;
    [SerializeField]
    private Slider zwSliderAbsolute;
    [SerializeField]
    private GameObject relativeModeParent;
    [SerializeField]
    private GameObject absoluteModeParent;

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
        positionText.text = $"<color=#808080>" +
        $"<color=#FF0000>x</color>: <color=#FF0000>{position.x:F2}</color>\n" +
        $"<color=#00FF00>y</color>: <color=#00FF00>{position.y:F2}</color>\n" +
        $"<color=#0080FF>z</color>: <color=#0080FF>{position.z:F2}</color>\n" +
        $"<color=#FFFF00>w</color>: <color=#FFFF00>{position.w:F2}</color></color>";
    }

    public void UpdateEulerRotationSliders(RotationEuler4 rotationEuler)
    {
        if (displayQuaternionPair)
        {
            return;
        }

        xwSlider.SetValueWithoutNotify(rotationEuler.xw);
        ywSlider.SetValueWithoutNotify(rotationEuler.yw);
        zwSlider.SetValueWithoutNotify(rotationEuler.zw);
        xySlider.SetValueWithoutNotify(rotationEuler.xy);
        xzSlider.SetValueWithoutNotify(rotationEuler.xz);
        yzSlider.SetValueWithoutNotify(rotationEuler.yz);
    }
    public void UpdateQuaternionPairRotationSliders(Rotation4 rotation)
    {
        if (!displayQuaternionPair)
        {
            return;
        }

        lwInput.SetTextWithoutNotify(rotation.leftQuaternion.w.ToString("F6"));
        lxInput.SetTextWithoutNotify(rotation.leftQuaternion.x.ToString("F6"));
        lyInput.SetTextWithoutNotify(rotation.leftQuaternion.y.ToString("F6"));
        lzInput.SetTextWithoutNotify(rotation.leftQuaternion.z.ToString("F6"));
        rwInput.SetTextWithoutNotify(rotation.rightQuaternion.w.ToString("F6"));
        rxInput.SetTextWithoutNotify(rotation.rightQuaternion.x.ToString("F6"));
        ryInput.SetTextWithoutNotify(rotation.rightQuaternion.y.ToString("F6"));
        rzInput.SetTextWithoutNotify(rotation.rightQuaternion.z.ToString("F6"));
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
        float pow = 2f;

        RotationEuler4 sliderRotationEuler = new(
            absoluteMode ? 0 : Mathf.Sign(xwSlider.value) * Mathf.Pow(Mathf.Abs(xwSlider.value), pow),
            absoluteMode ? 0 : Mathf.Sign(ywSlider.value) * Mathf.Pow(Mathf.Abs(ywSlider.value), pow),
            absoluteMode ? 0 : Mathf.Sign(zwSlider.value) * Mathf.Pow(Mathf.Abs(zwSlider.value), pow),
            Mathf.Sign(xySlider.value) * Mathf.Pow(Mathf.Abs(xySlider.value), pow),
            Mathf.Sign(xzSlider.value) * Mathf.Pow(Mathf.Abs(xzSlider.value), pow),
            Mathf.Sign(yzSlider.value) * Mathf.Pow(Mathf.Abs(yzSlider.value), pow));

        cameraState.cameraRotation.continuousRotationDelta = sliderRotationEuler;

        if (absoluteMode)
        {
            cameraState.absoluteModeRotationAngles = new Vector3(
                xwSliderAbsolute.value,
                ywSliderAbsolute.value,
                zwSliderAbsolute.value);

            cameraState.UpdateRotationDelta(RotationEuler4.zero);
        }
    }
    public void OnRotationSliderEndDrag()
    {
        if (!lockSliders)
        {
            cameraState.cameraRotation.continuousRotationDelta = RotationEuler4.zero;
            UpdateEulerRotationSliders(RotationEuler4.zero);
        }
    }

    public void SwitchDisplay()
    {
        displayQuaternionPair = !displayQuaternionPair;

        cameraState.cameraRotation.continuousRotationDelta = RotationEuler4.zero;

        UpdateDisplay();
    }
    private void UpdateDisplay()
    {
        eulerAnglesParent.SetActive(!displayQuaternionPair);
        quaternionPairParent.SetActive(displayQuaternionPair);

        switchButtonText.text = displayQuaternionPair ? "Quaternion Pair" : "Euler Angles";

        UpdateEulerRotationSliders(RotationEuler4.zero);
        UpdateQuaternionPairRotationSliders(cameraState.rotation);

        if (!displayQuaternionPair)
        {
            ReenableGlobalInput();
        }
    }

    public void ResetRotation()
    {
        cameraState.ResetRotation();

        cameraState.hypersceneRenderer.ResetRotation();
    }

    public void OnDropdownValueChanged(int id)
    {
        int value = hypersceneDropdown.value;

        HypersceneRenderer.HypersceneOption selectedOption = (HypersceneRenderer.HypersceneOption)value;
        cameraState.hypersceneRenderer.LoadHyperscene(selectedOption);
    }

    public void SwitchEulerSliderLock()
    {
        lockSliders = !lockSliders;

        lockSlidersImage.sprite = lockSliders ? lockSliderEnabledSprite : lockSliderDisabledSprite;
        lockSlidersImage.color = lockSliders ? new Color(1f, 0.5f, 1f) : new Color(160f / 256f, 160f / 256f, 160f / 256f);

        OnRotationSliderEndDrag();
    }

    public void SwitchAbsoluteRelativeMode()
    {
        absoluteMode = !absoluteMode;

        cameraState.ResetRotation();
        cameraState.absoluteMode = absoluteMode;

        absoluteModeImage.sprite = absoluteMode ? absoluteModeEnabledSprite : absoluteModeDisabledSprite;
        absoluteModeImage.color = absoluteMode ? new Color(1f, 0.5f, 1f) : new Color(160f / 256f, 160f / 256f, 160f / 256f);

        absoluteModeParent.SetActive(absoluteMode);
        relativeModeParent.SetActive(!absoluteMode);
    }
    public void OnAbsoluteRotationChange(Vector3 newValues)
    {
        xwSliderAbsolute.SetValueWithoutNotify(Helpers.Mod(newValues.x + Mathf.PI, Mathf.PI * 2f) - Mathf.PI);
        ywSliderAbsolute.SetValueWithoutNotify(Helpers.Mod(newValues.y + Mathf.PI / 2f, Mathf.PI) - Mathf.PI / 2f);
        zwSliderAbsolute.SetValueWithoutNotify(Helpers.Mod(newValues.z + Mathf.PI / 2f, Mathf.PI) - Mathf.PI / 2f);
    }
}
