using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public CameraPosition instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI positionText;
    [SerializeField]
    private TextMeshProUGUI rotationText;

    private CameraMovement cameraMovement;
    private CameraRotation cameraRotation;

    public delegate void OnValuesUpdate();
    public OnValuesUpdate onValuesUpdate;

    private void Awake()
    {
        instance = this;

        cameraMovement = GetComponent<CameraMovement>();
        cameraMovement.onPositionUpdate += OnPositionOrRotationUpdate;

        cameraRotation = GetComponent<CameraRotation>();
        cameraRotation.onRotationUpdate += OnPositionOrRotationUpdate;

        this.onValuesUpdate += OnValuesUpdateSelf;
    }
    private void Start()
    {

    }

    public Vector4 position = new Vector4(0,0,0,0);
    public Rotation4 rotation = new Rotation4(0,0,0,0,0,0);

    private void OnPositionOrRotationUpdate()
    {
        onValuesUpdate();
    }

    private void OnValuesUpdateSelf()
    {
        positionText.text = 
            $"x: {position.x}\n" +
            $"y: {position.y}\n" +
            $"z: {position.z}\n" +
            $"w: {position.w}";
        rotationText.text =
            $"xw: {rotation.xw * Mathf.Rad2Deg}°\n" +
            $"yw: {rotation.yw * Mathf.Rad2Deg}°\n" +
            $"zw: {rotation.zw * Mathf.Rad2Deg}°\n" +
            $"xy: {rotation.xy * Mathf.Rad2Deg}°\n" +
            $"xz: {rotation.xz * Mathf.Rad2Deg}°\n" +
            $"yz: {rotation.yz * Mathf.Rad2Deg}°";
    }
}
