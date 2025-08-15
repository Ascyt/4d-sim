using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum VideoRotationPlanesState
{
    Start,
    ShowRotateObject,
    RotateAroundXAxis,
    RotateAroundYAxis,
    RotateAroundZAxis,
    StopRotation,
    ShowXYPlane,
    RotateInXYPlane,
    ShowXZPlane,
    ShowYZPlane,
    UnhighlightPlanes,
    RotateInXYPlaneHighlightZAxis,
    RotateInXZPlaneHighlightYAxis,
    RotateInYZPlaneHighlightXAxis,
    End
}

[RequireComponent(typeof(Camera))]
public class VideoRotationPlanes : AnimatedStateMachine<VideoRotationPlanesState>
{
    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));

    private readonly Dictionary<VideoRotationPlanesState, float> _autoSkipStates = new()
    {
    };
    protected override Dictionary<VideoRotationPlanesState, float> AutoSkipStates => _autoSkipStates;

    [SerializeField]
    private GameObject rotatingObject;
    [SerializeField]
    private MeshRenderer xAxisRenderer;
    [SerializeField]
    private MeshRenderer xAxisArrowRenderer;
    [SerializeField]
    private MeshRenderer yAxisRenderer;
    [SerializeField]
    private MeshRenderer yAxisArrowRenderer;
    [SerializeField]
    private MeshRenderer zAxisRenderer;
    [SerializeField]
    private MeshRenderer zAxisArrowRenderer;

    [SerializeField]
    private GameObject xyPlaneObject;
    [SerializeField]
    private MeshRenderer xyPlaneRenderer;
    [SerializeField]
    private GameObject xzPlaneObject;
    [SerializeField] 
    private MeshRenderer xzPlaneRenderer;
    [SerializeField]
    private GameObject yzPlaneObject;
    [SerializeField]
    private MeshRenderer yzPlaneRenderer;

    private const float ROTATION_SPEED = 1f / 8f;
    private Color START_PLANE_COLOR => new Color(.125f, .125f, .125f, .5f);
    private Color END_PLANE_COLOR => new Color(1f, 1f, 1f, .5f);

    private float rotationResidue = 0f;

    protected override void OnEnterState(VideoRotationPlanesState state)
    {
        switch (state)
        {
            case VideoRotationPlanesState.ShowRotateObject:
                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    rotatingObject.transform.localScale = Mathf.Lerp(0f, .5f, fadingValue) * Vector3.one;
                });
                return;

            case VideoRotationPlanesState.RotateAroundXAxis:
                HighlightXAxis();
                RotateObjectEuler(new Vector3(360f, 0f, 0f));
                return;

            case VideoRotationPlanesState.RotateAroundYAxis:
                UnhighlightXAxis();
                HighlightYAxis();
                RotateObjectEuler(new Vector3(0f, 360f, 0f));
                return;

            case VideoRotationPlanesState.RotateAroundZAxis:
                UnhighlightYAxis();
                HighlightZAxis();
                RotateObjectEuler(new Vector3(0f, 0f, 360f));
                return;

            case VideoRotationPlanesState.StopRotation:
                UnhighlightZAxis();
                return;

            case VideoRotationPlanesState.ShowXYPlane:
                HighlightXYPlane();
                HighlightXAxis();
                HighlightYAxis();

                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    xyPlaneObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, fadingValue);
                });
                return;

            case VideoRotationPlanesState.RotateInXYPlane:
                RotateObjectEuler(new Vector3(0f, 0f, 360f));
                return;

            case VideoRotationPlanesState.ShowXZPlane:
                UnhighlightXYPlane();
                UnhighlightYAxis();
                HighlightXZPlane();
                HighlightZAxis();

                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    xzPlaneObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, fadingValue);
                });

                RotateObjectEuler(new Vector3(0f, 360f, 0f));
                return;

            case VideoRotationPlanesState.ShowYZPlane:
                UnhighlightXZPlane();
                UnhighlightXAxis();
                HighlightYZPlane();
                HighlightYAxis();

                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    yzPlaneObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, fadingValue);
                });

                RotateObjectEuler(new Vector3(360f, 0f, 0f));
                return;

            case VideoRotationPlanesState.UnhighlightPlanes:
                UnhighlightYZPlane();
                UnhighlightYAxis();
                UnhighlightZAxis();
                return;

            case VideoRotationPlanesState.RotateInXYPlaneHighlightZAxis:
                HighlightZAxis();
                HighlightXYPlane();

                RotateObjectEuler(new Vector3(0f, 0f, 360f));
                return;

            case VideoRotationPlanesState.RotateInXZPlaneHighlightYAxis:
                UnhighlightXYPlane();
                UnhighlightZAxis();
                HighlightXZPlane();
                HighlightYAxis();

                RotateObjectEuler(new Vector3(0f, 360f, 0f));
                return;

            case VideoRotationPlanesState.RotateInYZPlaneHighlightXAxis:
                UnhighlightXZPlane();
                UnhighlightYAxis();
                HighlightYZPlane();
                HighlightXAxis();

                RotateObjectEuler(new Vector3(360f, 0f, 0f));
                return;

            case VideoRotationPlanesState.End:
                UnhighlightYZPlane();
                UnhighlightXAxis();
                return;
        }
    }

    protected override void BeforeExitState(VideoRotationPlanesState state)
    {

    }

    private void RotateObjectEuler(Vector3 euler)
    {
        bool isStarted = false;

        Quaternion startRotation = Quaternion.identity;
        Quaternion fadeInEndRotation = Quaternion.identity;
        Fade(DefaultFading.WithEasingIO(Easing.IO.In).WithDelay(rotationResidue), (fadingValueA, isExitA) =>
        {
            if (!isStarted)
            {
                startRotation = rotatingObject.transform.rotation;
                // Rotation is half of the per-second rotation speed to adjust for integration over time.
                fadeInEndRotation = Quaternion.Euler(euler * ROTATION_SPEED / 2f) * startRotation; 
                isStarted = true;
            }

            rotatingObject.transform.rotation = Quaternion.Slerp(startRotation, fadeInEndRotation, fadingValueA);

            if (isExitA)
            {
                OnStateUpdate((float deltaTime, bool isExitB) =>
                {
                    rotatingObject.transform.rotation = Quaternion.Euler(ROTATION_SPEED * deltaTime * euler) * rotatingObject.transform.rotation;

                    if (isExitB)
                    {
                        Quaternion fadeOutStartRotation = rotatingObject.transform.rotation;
                        Quaternion endRotation = Quaternion.Euler(euler * ROTATION_SPEED / 2f) * fadeOutStartRotation;

                        rotationResidue = 1f;

                        Fade(DefaultFading.WithEasingIO(Easing.IO.Out), (fadingValueB, isExitC) =>
                        {
                            rotatingObject.transform.rotation = Quaternion.Slerp(fadeOutStartRotation, endRotation, fadingValueB);

                            rotationResidue = 1f - fadingValueB;

                            if (isExitC)
                            {
                                rotationResidue = 0f;
                            }
                        });
                    }
                });
            }
        }, runWhileOnDelay: false);
    }

    private void HighlightXYPlane()
    {
        FadePlane(xyPlaneRenderer, false);
    }
    private void UnhighlightXYPlane()
    {
        FadePlane(xyPlaneRenderer, true);
    }
    private void HighlightXZPlane()
    {
        FadePlane(xzPlaneRenderer, false);
    }
    private void UnhighlightXZPlane()
    {
        FadePlane(xzPlaneRenderer, true);
    }
    private void HighlightYZPlane()
    {
        FadePlane(yzPlaneRenderer, false);
    }
    private void UnhighlightYZPlane()
    {
        FadePlane(yzPlaneRenderer, true);
    }
    private void FadePlane(MeshRenderer planeRenderer, bool reverse)
    {
        Color startColor = START_PLANE_COLOR;
        Color endColor = END_PLANE_COLOR;

        if (reverse)
        {
            (startColor, endColor) = (endColor, startColor);
        }

        Fade(DefaultFading.WithDelay(rotationResidue * (reverse ? 0f : 1f)), (fadingValue, isExit) =>
        {
            Color color = Color.Lerp(startColor, endColor, fadingValue);
            planeRenderer.material.color = color;
        });
    }

    private void HighlightXAxis()
    {
        AxisColorGeneric(xAxisRenderer, xAxisArrowRenderer, new Color(.5f, 0f, 0f, 1f), new Color(1f, 0f, 0f, 1f), ignoreRotationResidue: false);
    } 
    private void UnhighlightXAxis()
    {
        AxisColorGeneric(xAxisRenderer, xAxisArrowRenderer, new Color(1f, 0f, 0f, 1f), new Color(.5f, 0f, 0f, 1f), ignoreRotationResidue: true);
    }
    private void HighlightYAxis()
    {
        AxisColorGeneric(yAxisRenderer, yAxisArrowRenderer, new Color(0f, .5f, 0f, 1f), new Color(0f, 1f, 0f, 1f), ignoreRotationResidue: false);
    }
    private void UnhighlightYAxis()
    {
        AxisColorGeneric(yAxisRenderer, yAxisArrowRenderer, new Color(0f, 1f, 0f, 1f), new Color(0f, .5f, 0f, 1f), ignoreRotationResidue: true);
    }
    private void HighlightZAxis()
    {
        AxisColorGeneric(zAxisRenderer, zAxisArrowRenderer, new Color(0f, .25f, .5f, 1f), new Color(0f, .5f, 1f, 1f), ignoreRotationResidue: false);
    }
    private void UnhighlightZAxis()
    {
        AxisColorGeneric(zAxisRenderer, zAxisArrowRenderer, new Color(0f, .5f, 1f, 1f), new Color(0f, .25f, .5f, 1f), ignoreRotationResidue: true);
    }
    private void AxisColorGeneric(MeshRenderer axisRenderer, MeshRenderer axisArrowRenderer, Color startColor, Color endColor, bool ignoreRotationResidue)
    {
        Fade(DefaultFading.WithDelay(rotationResidue * (ignoreRotationResidue ? 0f : 1f)), (fadingValue, isExit) =>
        {
            Color color = Color.Lerp(startColor, endColor, fadingValue);

            axisRenderer.material.color = color;
            axisArrowRenderer.material.color = color;
        });
    }

    private void Awake()
    {
        xAxisRenderer.sharedMaterial = new Material(xAxisRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };
        xAxisArrowRenderer.sharedMaterial = new Material(xAxisArrowRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };

        yAxisRenderer.sharedMaterial = new Material(yAxisRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };
        yAxisArrowRenderer.sharedMaterial = new Material(yAxisArrowRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };

        zAxisRenderer.sharedMaterial = new Material(yAxisRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };
        zAxisArrowRenderer.sharedMaterial = new Material(yAxisArrowRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };

        xyPlaneRenderer.sharedMaterial = new Material(xyPlaneRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };
        xzPlaneRenderer.sharedMaterial = new Material(xzPlaneRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };
        yzPlaneRenderer.sharedMaterial = new Material(yzPlaneRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave };
    }

    protected override void OnStart()
    {
        xAxisRenderer.material.color = new Color(.5f, 0f, 0f, 1f);
        xAxisArrowRenderer.material.color = new Color(.5f, 0f, 0f, 1f);
        yAxisRenderer.material.color = new Color(0f, .5f, 0f, 1f);
        yAxisArrowRenderer.material.color = new Color(0f, .5f, 0f, 1f);
        zAxisRenderer.material.color = new Color(0f, .25f, .5f, 1f);
        zAxisArrowRenderer.material.color = new Color(0f, .25f, .5f, 1f);

        rotatingObject.transform.localScale = Vector3.zero;
        rotatingObject.transform.rotation = Quaternion.identity;

        rotationResidue = 0f;

        xyPlaneRenderer.material.color = START_PLANE_COLOR;
        xzPlaneRenderer.material.color = START_PLANE_COLOR;
        yzPlaneRenderer.material.color = START_PLANE_COLOR;

        xyPlaneObject.transform.localScale = Vector3.zero;
        xzPlaneObject.transform.localScale = Vector3.zero;
        yzPlaneObject.transform.localScale = Vector3.zero;
    }
}
