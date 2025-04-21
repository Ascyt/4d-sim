using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CameraPosition))]
public class CameraMovement : MonoBehaviour
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

    private CameraPosition cameraPos;

    public delegate void OnPositionUpdate(Vector4 positionDelta);
    public OnPositionUpdate onPositionUpdate;

    private void Awake()
    {
        cameraPos = GetComponent<CameraPosition>();

        MoveRight       = cameraPos.useDvorak ? KeyCode.E       : KeyCode.D;
        MoveLeft        = cameraPos.useDvorak ? KeyCode.A       : KeyCode.A;
        MoveUp          = cameraPos.useDvorak ? KeyCode.Period  : KeyCode.E;
        MoveDown        = cameraPos.useDvorak ? KeyCode.Quote   : KeyCode.Q;
        MoveForwards    = cameraPos.useDvorak ? KeyCode.Comma   : KeyCode.W;
        MoveBackwards   = cameraPos.useDvorak ? KeyCode.O       : KeyCode.S;
        MoveAna         = KeyCode.Space;
        MoveKata        = KeyCode.LeftShift;
    }

    private void Update()
    {
        float speed = movementSpeed * Time.deltaTime;
        bool positionUpdated = false;

        Vector4 delta = Vector4.zero;

        if (Input.GetKey(cameraPos.RotationMovementSwitch ? MoveRight : cameraPos.cameraRotation.RotateXWPos))
        {
            delta += new Vector4(speed, 0, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(cameraPos.RotationMovementSwitch ? MoveLeft : cameraPos.cameraRotation.RotateXWNeg))
        {
            delta += new Vector4(-speed, 0, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(cameraPos.RotationMovementSwitch ? MoveUp : cameraPos.cameraRotation.RotateYWPos))
        {
            delta += new Vector4(0, speed, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(cameraPos.RotationMovementSwitch ? MoveDown : cameraPos.cameraRotation.RotateYWNeg))
        {
            delta += new Vector4(0, -speed, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(cameraPos.RotationMovementSwitch ? MoveForwards : cameraPos.cameraRotation.RotateZWPos))
        {
            delta += new Vector4(0, 0, speed, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(cameraPos.RotationMovementSwitch ? MoveBackwards : cameraPos.cameraRotation.RotateZWNeg))
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
