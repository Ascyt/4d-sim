using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CameraPosition))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    public float movementSpeed;

    private const bool USE_DVORAK = true;
    public static readonly KeyCode MoveRight        = USE_DVORAK ? KeyCode.E : KeyCode.D;
    public static readonly KeyCode MoveLeft         = USE_DVORAK ? KeyCode.A : KeyCode.A;
    public static readonly KeyCode MoveUp           = USE_DVORAK ? KeyCode.Period : KeyCode.E;
    public static readonly KeyCode MoveDown         = USE_DVORAK ? KeyCode.Quote : KeyCode.Q; 
    public static readonly KeyCode MoveForwards     = USE_DVORAK ? KeyCode.Comma : KeyCode.W;
    public static readonly KeyCode MoveBackwards    = USE_DVORAK ? KeyCode.O : KeyCode.S;   
    public static readonly KeyCode MoveAna          = KeyCode.Space;   
    public static readonly KeyCode MoveKata         = KeyCode.LeftShift;

    private CameraPosition cameraPos;

    public delegate void OnPositionUpdate(Vector4 positionDelta);
    public OnPositionUpdate onPositionUpdate;

    private void Awake()
    {
        cameraPos = GetComponent<CameraPosition>();
    }

    private void Update()
    {
        float speed = movementSpeed * Time.deltaTime;
        bool positionUpdated = false;

        Vector4 delta = Vector4.zero;

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveRight : CameraRotation.RotateXWPos))
        {
            delta += new Vector4(speed, 0, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveLeft : CameraRotation.RotateXWNeg))
        {
            delta += new Vector4(-speed, 0, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveUp : CameraRotation.RotateYWPos))
        {
            delta += new Vector4(0, speed, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveDown : CameraRotation.RotateYWNeg))
        {
            delta += new Vector4(0, -speed, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveForwards : CameraRotation.RotateZWPos))
        {
            delta += new Vector4(0, 0, speed, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(!cameraPos.movementRotationSwitch ? MoveBackwards : CameraRotation.RotateZWNeg))
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
