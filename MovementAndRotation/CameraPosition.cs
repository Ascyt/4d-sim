using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public CameraPosition instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI positionText;

    private CameraMovement cameraMovement;

    public delegate void OnValuesUpdate();
    public OnValuesUpdate onValuesUpdate;

    private void Awake()
    {
        instance = this;

        cameraMovement = GetComponent<CameraMovement>();
        cameraMovement.onPositionUpdate += OnPositionUpdate;

        this.onValuesUpdate += OnValuesUpdateSelf;
    }
    private void Start()
    {
        OnPositionUpdate();
    }

    public Vector4 position = new Vector4(0,0,0,0);
    public Rotation4 rotation = new Rotation4(0,0,0,0,0,0);

    private void OnPositionUpdate()
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
    }
}
