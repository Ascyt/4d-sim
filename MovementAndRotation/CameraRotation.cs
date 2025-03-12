using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    [SerializeField]
    public float movementSpeed;

    private const bool USE_DVORAK = false;

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
        bool rotationUpdated = false;

        if (rotationUpdated)
        {
            onPositionUpdate();
        }
    }
}