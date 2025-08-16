using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Rotation control for the camera using keyboard.
/// </summary>
[RequireComponent(typeof(CameraState))]
public class CameraRotation : MonoBehaviour
{
    private CameraState cameraState;

    [SerializeField]
    private float keyRotationSpeed;
    [SerializeField]
    private float sliderRotationSpeed;

    [HideInInspector]
    public KeyCode RotateXWPos;
    [HideInInspector]
    public KeyCode RotateXWNeg;
    [HideInInspector]
    public KeyCode RotateYWPos;
    [HideInInspector]
    public KeyCode RotateYWNeg;
    [HideInInspector]
    public KeyCode RotateZWPos;
    [HideInInspector]
    public KeyCode RotateZWNeg;

    [HideInInspector]
    public RotationEuler4 continuousRotationDelta = RotationEuler4.zero;

    private void Awake()
    {
        cameraState = GetComponent<CameraState>();

        RotateXWPos = cameraState.useDvorak ? KeyCode.N : KeyCode.L;
        RotateXWNeg = cameraState.useDvorak ? KeyCode.H : KeyCode.J;
        RotateYWPos = cameraState.useDvorak ? KeyCode.R : KeyCode.O;
        RotateYWNeg = cameraState.useDvorak ? KeyCode.G : KeyCode.U;
        RotateZWPos = cameraState.useDvorak ? KeyCode.C : KeyCode.I;
        RotateZWNeg = cameraState.useDvorak ? KeyCode.T : KeyCode.K;
    }

    private void Update()
    {
        float speed = keyRotationSpeed * Time.deltaTime;
        RotationEuler4 rotationDelta = continuousRotationDelta * (sliderRotationSpeed * Mathf.PI * 2 * Time.deltaTime);
        bool rotationUpdated = false;

        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateXWPos : cameraState.cameraMovement.MoveRight))
        {
            rotationDelta.xw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateXWNeg : cameraState.cameraMovement.MoveLeft))
        {
            rotationDelta.xw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateYWPos : cameraState.cameraMovement.MoveUp))
        {
            rotationDelta.yw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateYWNeg : cameraState.cameraMovement.MoveDown))
        {
            rotationDelta.yw -= speed;
            rotationUpdated = true;
        }

        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateZWPos : cameraState.cameraMovement.MoveForwards))
        {
            rotationDelta.zw += speed;
            rotationUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? RotateZWNeg : cameraState.cameraMovement.MoveBackwards))
        {
            rotationDelta.zw -= speed;
            rotationUpdated = true;
        }

        if (!rotationUpdated)
        {
            rotationUpdated = !continuousRotationDelta.IsZero();
        }

        if (rotationUpdated)
        {
            cameraState.UpdateRelativeRotationDelta(rotationDelta);
        }
    }
}