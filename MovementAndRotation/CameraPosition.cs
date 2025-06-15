using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CameraState))]
public class CameraPosition : MonoBehaviour
{
    [SerializeField]
    public float movementSpeed;

    [HideInInspector]
    public KeyCode MoveRight;
    [HideInInspector]
    public KeyCode MoveLeft;
    [HideInInspector]
    public KeyCode MoveUp;
    [HideInInspector]
    public KeyCode MoveDown;
    [HideInInspector]
    public KeyCode MoveForwards;
    [HideInInspector]
    public KeyCode MoveBackwards;
    [HideInInspector]
    public KeyCode MoveAna;
    [HideInInspector]
    public KeyCode MoveKata;     

    private CameraState cameraState;

    public delegate void OnPositionUpdate(Vector4 positionDelta);
    public OnPositionUpdate onPositionUpdate;

    private void Awake()
    {
        cameraState = GetComponent<CameraState>();

        MoveRight       = cameraState.useDvorak ? KeyCode.E       : KeyCode.D;
        MoveLeft        = cameraState.useDvorak ? KeyCode.A       : KeyCode.A;
        MoveUp          = cameraState.useDvorak ? KeyCode.Period  : KeyCode.E;
        MoveDown        = cameraState.useDvorak ? KeyCode.Quote   : KeyCode.Q;
        MoveForwards    = cameraState.useDvorak ? KeyCode.Comma   : KeyCode.W;
        MoveBackwards   = cameraState.useDvorak ? KeyCode.O       : KeyCode.S;
        MoveAna         = KeyCode.Space;
        MoveKata        = KeyCode.LeftShift;
    }

    private void Update()
    {
        float speed = movementSpeed * Time.deltaTime;
        bool positionUpdated = false;

        Vector4 delta = Vector4.zero;

        if (Input.GetKey(cameraState.RotationMovementSwitch ? MoveRight : cameraState.cameraRotation.RotateXWPos))
        {
            delta += new Vector4(speed, 0, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? MoveLeft : cameraState.cameraRotation.RotateXWNeg))
        {
            delta += new Vector4(-speed, 0, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(cameraState.RotationMovementSwitch ? MoveUp : cameraState.cameraRotation.RotateYWPos))
        {
            delta += new Vector4(0, speed, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? MoveDown : cameraState.cameraRotation.RotateYWNeg))
        {
            delta += new Vector4(0, -speed, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(cameraState.RotationMovementSwitch ? MoveForwards : cameraState.cameraRotation.RotateZWPos))
        {
            delta += new Vector4(0, 0, speed, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(cameraState.RotationMovementSwitch ? MoveBackwards : cameraState.cameraRotation.RotateZWNeg))
        {
            delta += new Vector4(0, 0, -speed, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(MoveAna))
        {
            delta += new Vector4(0, 0, 0, speed);
            positionUpdated = true;
        }
        if (Input.GetKey(MoveKata))
        {
            delta += new Vector4(0, 0, 0, -speed);
            positionUpdated = true;
        }

        /*if (cameraPos.platformerMode)
        {
            //rotatedDelta = new Vector4(rotatedDelta.x, rotatedDelta.y, rotatedDelta.z, rotatedDelta.w);
        }*/

        if (positionUpdated)
        {
            onPositionUpdate(delta);
        }
    }
}
