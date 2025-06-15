using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private TextMeshProUGUI movementRotationSwitchText;

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

    public void UpdateRotationText(Rotation4 rotation)
    {
        rotationText.text =
            $"xw: {rotation.xw * Mathf.Rad2Deg}°\n" +
            $"yw: {rotation.yw * Mathf.Rad2Deg}°\n" +
            $"zw: {rotation.zw * Mathf.Rad2Deg}°\n" +
            $"xy: {rotation.xy * Mathf.Rad2Deg}°\n" +
            $"xz: {rotation.xz * Mathf.Rad2Deg}°\n" +
            $"yz: {rotation.yz * Mathf.Rad2Deg}°";
    }

    public void UpdateRotationSliderValues(Rotation4 rotation)
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
        Rotation4 sliderRotation = new Rotation4(xwSlider.value, ywSlider.value, zwSlider.value, xySlider.value, xzSlider.value, yzSlider.value);
        Rotation4 rotationDelta = sliderRotation - cameraState.rotation;

        cameraState.UpdateRotation(rotationDelta);
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
}
