using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VideoMovementState
{
    Start,
    ShowAxes,
    MoveRight,
    HighlightBackFace,
    UnhighlightBackFace,
    BackToOrigin1,
    MoveLeft,
    BackToOrigin2,
    MoveUpAndDown,
    MoveForwards,
    MoveBackwards,
    End
}

[RequireComponent(typeof(Camera))]
public class VideoMovement : AnimatedStateMachine<VideoMovementState>
{
    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));

    private readonly Dictionary<VideoMovementState, float> _autoSkipStates = new()
    {
    };
    protected override Dictionary<VideoMovementState, float> AutoSkipStates => _autoSkipStates;

    [SerializeField]
    private GameObject axesObject;
    [SerializeField]
    private GameObject xAxisParentObject;
    [SerializeField]
    private MeshRenderer xAxisRenderer;
    [SerializeField]
    private MeshRenderer xAxisArrowRenderer;
    [SerializeField]
    private GameObject yAxisParentObject;
    [SerializeField]
    private MeshRenderer yAxisRenderer;
    [SerializeField]
    private MeshRenderer yAxisArrowRenderer;

    [SerializeField]
    private GameObject highlightableCubePrefab;

    private float startMoveTime = 0f;
    private Vector3 startMovePosition = Vector3.zero;

    private GameObject highlightedFaceObject;

    protected override void OnEnterState(VideoMovementState state)
    {
        switch (state)
        {
            case VideoMovementState.ShowAxes:
                Vector3 axesScale = axesObject.transform.localScale;
                axesObject.transform.localScale = Vector3.zero;
                axesObject.SetActive(true);

                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    axesObject.transform.localScale = Vector3.Lerp(Vector3.zero, axesScale, fadingValue);
                });
                return;
            case VideoMovementState.MoveRight:
                startMoveTime = Time.time;

                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    Color color = new(Mathf.Lerp(.5f, 1f, fadingValue), 0f, 0f, 1f);

                    xAxisRenderer.material.color = color;
                    xAxisArrowRenderer.material.color = color;
                });
                return;
            case VideoMovementState.HighlightBackFace:
                if (highlightedFaceObject != null)
                {
                    Destroy(highlightedFaceObject);
                }
                highlightedFaceObject = Instantiate(highlightableCubePrefab, new Vector3(0f, 0f, .5f), Quaternion.identity);
                highlightedFaceObject.transform.localScale = new Vector3(1f, 1f, 0f);
                highlightedFaceObject.SetActive(true);

                MeshRenderer mr = highlightedFaceObject.GetComponent<MeshRenderer>();
                mr.material = new Material(mr.material);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        Material mat = mr.material;
                        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, fadingValue * .5f);
                    });
                return;

            case VideoMovementState.UnhighlightBackFace:
                MeshRenderer mr1 = highlightedFaceObject.GetComponent<MeshRenderer>();
                mr1.material = new Material(mr1.material);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        Material mat = mr1.material;
                        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, (1f - fadingValue) * .5f);

                        if (isExit)
                        {
                            Destroy(mr1.material);
                            Destroy(highlightedFaceObject);
                        }
                    });

                return;

            case VideoMovementState.BackToOrigin1:
                BackToOrigin();

                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    Color color = new(Mathf.Lerp(1f, .5f, fadingValue), 0f, 0f, 1f);
                    xAxisRenderer.material.color = color;
                    xAxisArrowRenderer.material.color = color;
                });
                return;

            case VideoMovementState.MoveLeft:
                startMoveTime = Time.time;

                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    Color color = new(Mathf.Lerp(.5f, 1f, fadingValue), 0f, 0f, 1f);
                    xAxisRenderer.material.color = color;
                    xAxisArrowRenderer.material.color = color;

                    xAxisParentObject.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(-1f, 1f, 1f), fadingValue);
                });
                return;

            case VideoMovementState.BackToOrigin2:
                BackToOrigin();
                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    Color color = new Color(Mathf.Lerp(1f, .5f, fadingValue), 0f, 0f, 1f);
                    xAxisRenderer.material.color = color;
                    xAxisArrowRenderer.material.color = color;

                    xAxisParentObject.transform.localScale = Vector3.Lerp(new Vector3(-1f, 1f, 1f), Vector3.one, fadingValue);
                });
                return;

            case VideoMovementState.MoveUpAndDown:
                Fade(DefaultFading, (fadingValue, isExit) =>
                {
                    Color color = new Color(0f, Mathf.Lerp(.5f, 1f, fadingValue), 0f, 1f);
                    yAxisRenderer.material.color = color;
                    yAxisArrowRenderer.material.color = color;
                });

                Fade(new Fading(2f, new Easing(Easing.Type.Sine, Easing.IO.InOut), 0f), 
                    (fadingValue, isExit) =>
                    {
                        transform.position = new Vector3(0f, fadingValue * .75f, -3f);

                        if (isExit)
                        {
                            float endPosY = transform.position.y;
                            Fade(new Fading(4f, new Easing(Easing.Type.Sine, Easing.IO.InOut), 0f),
                                (fadeValue1, isExit1) =>
                                {
                                    transform.position = new Vector3(0f, Mathf.Lerp(endPosY, -endPosY, fadeValue1), -3f);

                                    if (isExit1)
                                    {
                                        Fade(new Fading(2f, new Easing(Easing.Type.Sine, Easing.IO.InOut), 0f),
                                            (fadeValue2, isExit2) =>
                                            { 
                                                transform.position = new Vector3(0f, Mathf.Lerp(-endPosY, 0f, fadeValue2), -3f);
                                            });

                                        Fade(DefaultFading, (fadingValue, isExit) =>
                                        {
                                            Color color = new Color(0f, Mathf.Lerp(1f, .5f, fadingValue), 0f, 1f);
                                            yAxisRenderer.material.color = color;
                                            yAxisArrowRenderer.material.color = color;

                                            yAxisParentObject.transform.localScale = Vector3.Lerp(new Vector3(1f, -1f, 1f), Vector3.one, fadingValue);
                                        });
                                    }
                                });


                            Fade(DefaultFading, (fadingValue, isExit) =>
                            {
                                yAxisParentObject.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1f, -1f, 1f), fadingValue);
                            });
                        }
                    });
                return;

            case VideoMovementState.MoveForwards:
                startMoveTime = Time.time;
                return;

            case VideoMovementState.MoveBackwards:
                startMovePosition = transform.position;
                startMoveTime = Time.time;
                return;
        }
    }

    private void BackToOrigin()
    {
        Vector3 startPosition = transform.position;

        Fade(DefaultFading, (value, isExit) =>
        {
            transform.position = Vector3.Lerp(startPosition, new Vector3(0f, 0f, -3f), value);
        });
    }

    protected override void BeforeExitState(VideoMovementState state)
    {

    }
    protected override void OnUpdate()
    {
        if (new VideoMovementState[] { VideoMovementState.MoveRight, VideoMovementState.HighlightBackFace, VideoMovementState.UnhighlightBackFace }.Contains(CurrentState))
        {
            transform.position = new Vector3((Time.time - startMoveTime) / 4f, 0f, -3f);
        }

        if (new VideoMovementState[] { VideoMovementState.MoveLeft }.Contains(CurrentState))
        {
            transform.position = new Vector3(-(Time.time - startMoveTime) / 4f, 0f, -3f);
        }

        if (new VideoMovementState[] { VideoMovementState.MoveForwards }.Contains(CurrentState))
        {
            transform.position = new Vector3(0f, 0f, -3f + (Time.time - startMoveTime) / 4f);
        }
        if (new VideoMovementState[] { VideoMovementState.MoveBackwards }.Contains(CurrentState))
        {
            transform.position = startMovePosition + new Vector3(0f, 0f, -(Time.time - startMoveTime) / 4f);
        }
    }

    private void Awake()
    {
        xAxisRenderer.sharedMaterial = new Material(xAxisRenderer.sharedMaterial);
        xAxisRenderer.sharedMaterial.hideFlags = HideFlags.DontSave;
        xAxisArrowRenderer.sharedMaterial = new Material(xAxisArrowRenderer.sharedMaterial);
        xAxisArrowRenderer.sharedMaterial.hideFlags = HideFlags.DontSave;

        yAxisRenderer.sharedMaterial = new Material(yAxisRenderer.sharedMaterial);
        yAxisRenderer.sharedMaterial.hideFlags = HideFlags.DontSave;
        yAxisArrowRenderer.sharedMaterial = new Material(yAxisArrowRenderer.sharedMaterial);
        yAxisArrowRenderer.sharedMaterial.hideFlags = HideFlags.DontSave;
    }

    protected override void OnStart()
    {
        axesObject.SetActive(false);   
        transform.position = new Vector3(0f, 0f, -3f);

        if (highlightedFaceObject != null)
        {
            Destroy(highlightedFaceObject);
            highlightedFaceObject = null;
        }

        xAxisParentObject.transform.localScale = Vector3.one;
        yAxisParentObject.transform.localScale = Vector3.one;

        xAxisRenderer.material.color = new Color(.5f, 0f, 0f, 1f);
        xAxisArrowRenderer.material.color = new Color(.5f, 0f, 0f, 1f);
        yAxisRenderer.material.color = new Color(0f, .5f, 0f, 1f);
        yAxisArrowRenderer.material.color = new Color(0f, .5f, 0f, 1f);
    }
}
