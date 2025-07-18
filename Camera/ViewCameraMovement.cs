using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Controls the 3D viewport's camera movement and rotation using mouse input.
/// </summary>
public class ViewCameraMovement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How much the actions should get divided by when the Alt key is pressed")]
    private float precisionChange;
    private float precision;

    private bool isRotating = false;
    private bool isRotatingAround = false;
    private bool isPanning = false;

    private void Start()
    {
        distanceFromTarget = defaultDistanceFromTarget;
        precision = 1;
    }
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        precision = Input.GetKey(KeyCode.LeftAlt) ? precisionChange : 1;


        if (Input.GetMouseButtonDown(0))
            isPanning = true;
        if (!Input.GetMouseButton(0))
            isPanning = false;

        if (Input.GetMouseButtonDown(1))
            isRotatingAround = true;
        if (!Input.GetMouseButton(1))
            isRotatingAround = false;

        if (Input.GetMouseButtonDown(2))
            isRotating = true;
        if (!Input.GetMouseButton(2))
            isRotating = false;

        MoveControls();
        RotationControls();
    }

    [Space]
    [SerializeField]
    [Tooltip("Speed at which the camera moves forward and backward when scrolling")]
    private float scrollSpeed;
    private void MoveControls()
    {
        Vector2 scrollDelta = Input.GetMouseButton(1) ? Vector2.zero : Input.mouseScrollDelta;

        Vector3 dir = new Vector3(scrollDelta.x * scrollSpeed, 0f, scrollDelta.y * scrollSpeed);

        if (dir != Vector3.zero)
            transform.Translate(dir * Time.deltaTime / precision);
    }

    [Space]
    [SerializeField]
    [Tooltip("Sensitivity for left-clicked first-person rotation")]
    private float rotationSensitivity;
    [SerializeField]
    [Tooltip("Sensitivity for right-clicked rotation around a point")]
    private float rotateAroundSensitivity;

    // to bring the mouse pointer back after rotation action
    private MousePosition.Point? mouseOrigin = null;

    [Space]
    [SerializeField]
    [Tooltip("Target point to show and rotate around while right-clicking")]
    private Transform target;
    [SerializeField]
    [Tooltip("Default distance between the camera and the target point")]
    private float defaultDistanceFromTarget;
    private float distanceFromTarget;
    [SerializeField]
    [Tooltip("Sensitivity for scrolling to change the distance between the camera and the target point")]
    private float distanceFromTargetScrollSensitivity;
    [SerializeField]
    [Tooltip("Minimum and maximum distance between the camera and the target point")]
    private Vector2 distanceFromTargetBounds;

    [Space]
    [SerializeField]
    [Tooltip("Sensitivity for the middle-clicked panning of the camera")]
    private float panSensitivity;
    private void RotationControls()
    {
        bool anyMouseDown = isRotating || isRotatingAround || isPanning;

        if (anyMouseDown)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                // lock the cursor
                mouseOrigin = MousePosition.GetCursorPosition();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else if (mouseOrigin != null) // null check is to make sure it only happens once
        {
            // unlock the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            MousePosition.SetCursorPosition(mouseOrigin ?? new MousePosition.Point(0, 0));

            mouseOrigin = null;
            return;
        }

        Vector2 mouseDelta = Camera.main.ScreenToViewportPoint(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        target.gameObject.SetActive(isRotatingAround);
        // Rotate the camera around the target point
        if (isRotatingAround)
        {
            SetDistanceFromTarget();

            // Apply the rotation
            float amount = rotateAroundSensitivity / precision;
            transform.RotateAround(target.position, Vector3.up, mouseDelta.x * amount);
            transform.RotateAround(target.position, transform.right, -mouseDelta.y * amount);

            // Move the target so it stays visually at the center of the camera
            target.position = transform.position + transform.forward * distanceFromTarget;
        }

        if (mouseDelta == Vector2.zero) // performance
            return;

        // Rotate camera along Y axis
        else if (isRotating)
        {
            float amount = rotationSensitivity / precision;
            transform.RotateAround(transform.position, transform.right, -mouseDelta.y * amount);
            transform.RotateAround(transform.position, Vector3.up, mouseDelta.x * amount);
        }

        // Move the camera on its XZ plane
        else if (isPanning)
        {
            float amount = -panSensitivity / precision;

            Vector3 move = new Vector3(mouseDelta.x, mouseDelta.y, 0) * amount;
            move = transform.TransformDirection(move);
            transform.Translate(move, Space.World);
        }
    }

    // Changes the distance from the target point if scrolled and reverts it to default if left-clicked
    private void SetDistanceFromTarget()
    {
        // When left-clicked, return to default
        if (Input.GetMouseButtonDown(0))
        {
            distanceFromTarget = defaultDistanceFromTarget;
            return;
        }

        float scrollDelta = Input.mouseScrollDelta.y * distanceFromTargetScrollSensitivity / precision;

        if (scrollDelta > 0)
            distanceFromTarget *= scrollDelta + 1;
        else
            distanceFromTarget /= -scrollDelta + 1;

        distanceFromTarget = Mathf.Clamp(distanceFromTarget, distanceFromTargetBounds.x, distanceFromTargetBounds.y);
    }
}
