using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CameraPosition))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    public float movementSpeed;

    public KeyCode MoveRight;
    public KeyCode MoveLeft;
    public KeyCode MoveUp;
    public KeyCode MoveDown;
    public KeyCode MoveForwards;
    public KeyCode MoveBackwards;
    public KeyCode MoveAna;
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

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveRight : cameraPos.cameraRotation.RotateXWPos))
        {
            delta += new Vector4(speed, 0, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveLeft : cameraPos.cameraRotation.RotateXWNeg))
        {
            delta += new Vector4(-speed, 0, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveUp : cameraPos.cameraRotation.RotateYWPos))
        {
            delta += new Vector4(0, speed, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveDown : cameraPos.cameraRotation.RotateYWNeg))
        {
            delta += new Vector4(0, -speed, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveForwards : cameraPos.cameraRotation.RotateZWPos))
        {
            delta += new Vector4(0, 0, speed, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveBackwards : cameraPos.cameraRotation.RotateZWNeg))
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

        if (cameraPos.platformerMode)
        {
            //rotatedDelta = new Vector4(rotatedDelta.x, rotatedDelta.y, rotatedDelta.z, rotatedDelta.w);
        }

        if (positionUpdated)
        {
            onPositionUpdate(delta);
        }
    }
}
