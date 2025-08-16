using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VideoRotationFirstpersonState
{
    Start,
    ShowAxes,
    RotateToOrigin,
    End
}

[RequireComponent(typeof(Camera))]
public class VideoRotationFirstperson : AnimatedStateMachine<VideoRotationFirstpersonState>
{
    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));

    private readonly Dictionary<VideoRotationFirstpersonState, float> _autoSkipStates = new()
    {
    };
    protected override Dictionary<VideoRotationFirstpersonState, float> AutoSkipStates => _autoSkipStates;

    [SerializeField]
    private GameObject axesObject;

    [SerializeField]
    private float rotationSpeed;

    protected override void OnEnterState(VideoRotationFirstpersonState state)
    {
        switch (state)
        {
            case VideoRotationFirstpersonState.ShowAxes:
                Vector3 axesScale = axesObject.transform.localScale;
                axesObject.transform.localScale = Vector3.zero;
                axesObject.SetActive(true);

                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    axesObject.transform.localScale = Vector3.Lerp(Vector3.zero, axesScale, fadingValue);
                });
                return;
        }
    }

    protected override void BeforeExitState(VideoRotationFirstpersonState state)
    {

    }
    protected override void OnUpdate()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation *= Quaternion.Euler(0f, 360f * rotationSpeed * Time.deltaTime, 0f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation *= Quaternion.Euler(0f, -360f * rotationSpeed * Time.deltaTime, 0f);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.rotation *= Quaternion.Euler(-360f * rotationSpeed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation *= Quaternion.Euler(360f * rotationSpeed * Time.deltaTime, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.rotation *= Quaternion.Euler(0f, 0f, 360f * rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.rotation *= Quaternion.Euler(0f, 0f, -360f * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.R))
        {
            Quaternion startRotation = transform.rotation;
            Fade(DefaultFading, (fadingValue, isExit) =>
            {
                transform.rotation = Quaternion.Lerp(startRotation, Quaternion.identity, fadingValue);
            });
        }
    }

    protected override void OnStart()
    {
        axesObject.SetActive(false);   
        transform.position = new Vector3(0f, 0f, -3f);
    }
}
