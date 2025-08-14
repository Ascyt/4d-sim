using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum VideoDimensionalityState
{
    Start,
    AddAxisPoint,
    XAxis,
    YAxis,
    AddReferencePoint,
    MoveReferencePointX,
    OrthographicToPerspective,
    ZAxis,
    AddZToText,
    TextXYZLabelToNumbers,
    MoveReferencePointZ,
    AddWToText,
    WAxis,
    RotateWAxisParentFirst,
    RotateWAxisParentSecond
}

[RequireComponent(typeof(Camera))]
public class VideoDimensionality : AnimatedStateMachine<VideoDimensionalityState>
{
    [SerializeField]
    private GameObject axisPoint;
    [SerializeField]
    private GameObject xAxisObject;
    [SerializeField]
    private GameObject yAxisObject;
    [SerializeField]
    private GameObject zAxisObject;
    [SerializeField]
    private GameObject wAxisObject;
    [SerializeField]
    private GameObject wAxisParent;
    [SerializeField]
    private GameObject referencePoint;
    [SerializeField]
    private TextMeshPro referencePointPositionText;

    private Camera cam;

    private TextMeshPro fadingText = null;

    private readonly Quaternion startWAxisRotation = Quaternion.Euler(-45, 0, 45);
    private readonly Quaternion firstWAxisRotation = Quaternion.Euler(0, 45, -45);
    private readonly Quaternion secondWAxisRotation = Quaternion.Euler(-130, -35, 60);

    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    private readonly Dictionary<VideoDimensionalityState, float> _autoSkipStates = new()
    {
        { VideoDimensionalityState.AddZToText, 1.5f }
    };
    protected override Dictionary<VideoDimensionalityState, float> AutoSkipStates => _autoSkipStates;

    protected override void OnEnterState(VideoDimensionalityState state)
    {
        switch (state)
        {
            case VideoDimensionalityState.Start:
                OnStart();
                return;

            case VideoDimensionalityState.AddAxisPoint:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        axisPoint.transform.localScale = 0.25f * fadingValue * Vector3.one;
                    });
                return;

            case VideoDimensionalityState.XAxis:
                xAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        xAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        xAxisObject.transform.localPosition = new Vector3(fadingValue * 4f, 0, 0);
                    });

                return;

            case VideoDimensionalityState.YAxis:
                yAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        yAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        yAxisObject.transform.localPosition = new Vector3(0, fadingValue * 4f, 0);
                    });
                return;

            case VideoDimensionalityState.AddReferencePoint:
                UpdateReferencePointPositionText(includeZ: false, fade: false);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        referencePoint.transform.localScale = 0.25f * fadingValue * Vector3.one;
                    });
                return;

            case VideoDimensionalityState.MoveReferencePointX:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        referencePoint.transform.position = new Vector3(1f + fadingValue * 2f, 1f, 0);
                        UpdateReferencePointPositionText(includeZ: false, fade: false);
                    });
                return;

            case VideoDimensionalityState.OrthographicToPerspective:
                Fade(new Fading(0.5f, new Easing(Easing.Type.Expo, Easing.IO.Out)),
                    (fadingValue, isExit) =>
                    {
                        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -3 * (99f * (1 - fadingValue) + 1));
                        cam.fieldOfView = 60f / (99f * (1 - fadingValue) + 1);
                    });
                return;

            case VideoDimensionalityState.ZAxis:
                zAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        zAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        zAxisObject.transform.localPosition = new Vector3(0, 0, fadingValue * 4f);
                    });
                return;

            case VideoDimensionalityState.AddZToText:
                UpdateReferencePointPositionText(includeZ: true, fade: true, customText: $"<color=\"grey\">" +
                    $"(<color=\"red\">x</color>" +
                    $", <color=\"green\">y</color>" +
                    $", <color=#0080FF>z</color>" +
                    ")");
                return;

            case VideoDimensionalityState.TextXYZLabelToNumbers:
                UpdateReferencePointPositionText(includeZ: true, fade: true);
                return;

            case VideoDimensionalityState.MoveReferencePointZ:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        referencePoint.transform.position = new Vector3(3f, 1f, fadingValue * 2f);
                        UpdateReferencePointPositionText(includeZ: true, fade: false);
                    });
                return;

            case VideoDimensionalityState.AddWToText:
                UpdateReferencePointPositionText(includeZ: true, fade: true, customText: $"<color=\"grey\">" +
                    $"(<color=red>x</color>" +
                    $", <color=green>y</color>" +
                    $", <color=#0080FF>z</color>" +
                    $", <color=yellow>w</color>)?");
                return;

            case VideoDimensionalityState.WAxis:
                wAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        wAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        wAxisObject.transform.localPosition = new Vector3(0, fadingValue * 4f, 0);
                    });
                return;

            case VideoDimensionalityState.RotateWAxisParentFirst:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        wAxisParent.transform.rotation = Quaternion.Slerp(startWAxisRotation, firstWAxisRotation, fadingValue);
                    });
                return;

            case VideoDimensionalityState.RotateWAxisParentSecond:
                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        wAxisParent.transform.rotation = Quaternion.Slerp(firstWAxisRotation, secondWAxisRotation, fadingValue);
                    });
                return;
        }
    }

    protected override void BeforeExitState(VideoDimensionalityState state)
    {
        if (fadingText != null)
        {
            Destroy(fadingText.gameObject);
            fadingText = null;
            referencePointPositionText.alpha = 1f;
        }
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    protected override void OnStart()
    {
        xAxisObject.SetActive(false);
        yAxisObject.SetActive(false);
        zAxisObject.SetActive(false);
        wAxisObject.SetActive(false);

        referencePoint.transform.localScale = Vector3.zero;
        axisPoint.transform.localScale = Vector3.zero;

        wAxisParent.transform.rotation = startWAxisRotation;

        if (fadingText != null)
        {
            Destroy(fadingText.gameObject);
            fadingText = null;
        }

        referencePoint.transform.position = new Vector3(1f, 1f, 0);
        UpdateReferencePointPositionText(includeZ: false, fade: false);

        cam.transform.SetPositionAndRotation(new Vector3(2, 1, -3 * 100f), Quaternion.identity);
        cam.fieldOfView = 60f / 100f;
    }

    private void UpdateReferencePointPositionText(bool includeZ = true, bool fade = false, string customText = null)
    {
        referencePointPositionText.alpha = 1f;

        if (fade)
        {
            if (fadingText != null)
            {
                Destroy(fadingText.gameObject);
            }
            fadingText = Instantiate(referencePointPositionText.gameObject, referencePointPositionText.transform.parent).GetComponent<TextMeshPro>();
            referencePointPositionText.alpha = 0f;

            Fade(DefaultFading,
                (fadingValue, isExit) =>
                {
                    if (fadingText == null)
                        return;
                    fadingText.transform.position = referencePointPositionText.transform.position;
                    fadingText.transform.localScale = referencePointPositionText.transform.localScale;
                    fadingText.alpha = 1f - fadingValue;
                    referencePointPositionText.alpha = fadingValue;
                    if (Mathf.Approximately(fadingValue, 1f))
                    {
                        Destroy(fadingText.gameObject);
                        fadingText = null;
                        referencePointPositionText.alpha = 1f;
                    }
                });
        }

        customText ??= $"<color=\"grey\">" +
            $"(<color=\"red\">{referencePoint.transform.position.x:F2}</color>" +
            $", <color=\"green\">{referencePoint.transform.position.y:F2}</color>" +
            (includeZ ? $", <color=#0080FF>{referencePoint.transform.position.z:F2}</color>" : "") +
            ")";

        referencePointPositionText.text = customText;
    }
}
