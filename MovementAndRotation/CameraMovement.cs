using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    public float movementSpeed;

    private const bool USE_DVORAK = false;
    private static KeyCode MoveRight        = USE_DVORAK ? KeyCode.E : KeyCode.D;
    private static KeyCode MoveLeft         = USE_DVORAK ? KeyCode.A : KeyCode.A;
    private static KeyCode MoveUp           = KeyCode.Space;
    private static KeyCode MoveDown         = KeyCode.LeftShift;
    private static KeyCode MoveForwards     = USE_DVORAK ? KeyCode.Comma : KeyCode.W;
    private static KeyCode MoveBackwards    = USE_DVORAK ? KeyCode.O : KeyCode.S;
    private static KeyCode MoveAna          = USE_DVORAK ? KeyCode.Period : KeyCode.E;
    private static KeyCode MoveKata         = USE_DVORAK ? KeyCode.Quote : KeyCode.Q;

    private CameraPosition cameraPos;

    public delegate void OnPositionUpdate();
    public OnPositionUpdate onPositionUpdate;

    private void Awake()
    {
        cameraPos = GetComponent<CameraPosition>();
    }

    private void Update()
    {
        float speed = movementSpeed * Time.deltaTime;
        bool positionUpdated = false;

        if (Input.GetKey(MoveRight))
        {
            cameraPos.position += new Vector4(speed, 0, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(MoveLeft))
        {
            cameraPos.position += new Vector4(-speed, 0, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(MoveUp))
        {
            cameraPos.position += new Vector4(0, speed, 0, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(MoveDown))
        {
            cameraPos.position += new Vector4(0, -speed, 0, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(MoveForwards))
        {
            cameraPos.position += new Vector4(0, 0, speed, 0);
            positionUpdated = true;
        }
        if (Input.GetKey(MoveBackwards))
        {
            cameraPos.position += new Vector4(0, 0, -speed, 0);
            positionUpdated = true;
        }

        if (Input.GetKey(MoveAna))
        {
            cameraPos.position += new Vector4(0, 0, 0, speed);
            positionUpdated = true;
        }
        if (Input.GetKey(MoveKata))
        {
            cameraPos.position += new Vector4(0, 0, 0, -speed);
            positionUpdated = true;
        }

        if (positionUpdated)
        {
            onPositionUpdate();
        }
    }
}
