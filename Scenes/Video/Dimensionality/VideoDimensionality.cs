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

    private readonly Fading _defaultFading = new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    protected override Fading DefaultFading => _defaultFading;
    private readonly Dictionary<VideoDimensionalityState, float> _autoSkipStates = new()
    {
        { VideoDimensionalityState.AddZToText, 1.5f }
    };
    private readonly Dictionary<VideoDimensionalityState, Fading[]> _additionalFadings = new()
    {
        { VideoDimensionalityState.OrthographicToPerspective, new[] { new Fading(0.5f, new Easing(Easing.Type.Expo, Easing.IO.Out)) } },
    };
    protected override Dictionary<VideoDimensionalityState, Fading[]> AdditionalFadings => _additionalFadings;
    protected override Dictionary<VideoDimensionalityState, float> AutoSkipStates => _autoSkipStates;

    protected override void OnEnterState(VideoDimensionalityState state)
    {
        switch (state)
        {
            case VideoDimensionalityState.Start:
                OnStart();
                return;

            case VideoDimensionalityState.AddAxisPoint:
                return;

            case VideoDimensionalityState.XAxis:
                xAxisObject.SetActive(true);
                return;

            case VideoDimensionalityState.YAxis:
                yAxisObject.SetActive(true);
                return;

            case VideoDimensionalityState.AddReferencePoint:
                UpdateReferencePointPositionText(includeZ: false, fade: false);
                return;

            case VideoDimensionalityState.MoveReferencePointX:
                return;

            case VideoDimensionalityState.ZAxis:
                zAxisObject.SetActive(true);
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
                UpdateReferencePointPositionText(includeZ: true, fade: false);
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
                return;

            case VideoDimensionalityState.RotateWAxisParentFirst:
                wAxisParent.transform.rotation = startWAxisRotation;
                return;

            case VideoDimensionalityState.RotateWAxisParentSecond:
                wAxisParent.transform.rotation = firstWAxisRotation;
                return;
        }
    }

    protected override void OnExitState(VideoDimensionalityState state)
    {
        if (fadingText != null)
        {
            Destroy(fadingText.gameObject);
            fadingText = null;
            referencePointPositionText.alpha = 1f;
        }
    }


    protected override void OnUpdateState(VideoDimensionalityState state, float fadingValue, float[] additionalFadingValues)
    {
        if (fadingText != null)
        {
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
        }

        switch (state)
        {
            case VideoDimensionalityState.Start:
                return;

            case VideoDimensionalityState.AddAxisPoint:
                axisPoint.transform.localScale = 0.25f * fadingValue * Vector3.one;
                return;

            case VideoDimensionalityState.XAxis:
                xAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                xAxisObject.transform.localPosition = new Vector3(fadingValue * 4f, 0, 0);
                return;

            case VideoDimensionalityState.YAxis:
                yAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                yAxisObject.transform.localPosition = new Vector3(0, fadingValue * 4f, 0);
                return;

            case VideoDimensionalityState.AddReferencePoint:
                referencePoint.transform.localScale = 0.25f * fadingValue * Vector3.one;
                return;

            case VideoDimensionalityState.MoveReferencePointX:
                referencePoint.transform.position = new Vector3(1f + fadingValue * 2f, 1f, 0);
                UpdateReferencePointPositionText(includeZ: false, fade: false);
                return;

            case VideoDimensionalityState.ZAxis:
                zAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                zAxisObject.transform.localPosition = new Vector3(0, 0, fadingValue * 4f);
                return;

            case VideoDimensionalityState.OrthographicToPerspective:
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -3 * (99f * (1 - additionalFadingValues[0]) + 1));
                cam.fieldOfView = 60f / (99f * (1 - additionalFadingValues[0]) + 1);
                return;

            case VideoDimensionalityState.AddZToText:
                return;

            case VideoDimensionalityState.TextXYZLabelToNumbers:
                return;

            case VideoDimensionalityState.MoveReferencePointZ:
                referencePoint.transform.position = new Vector3(3f, 1f, fadingValue * 2f);
                UpdateReferencePointPositionText(includeZ: true, fade: false);
                return;

            case VideoDimensionalityState.AddWToText:
                return;

            case VideoDimensionalityState.WAxis:
                wAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                wAxisObject.transform.localPosition = new Vector3(0, fadingValue * 4f, 0);
                return;

            case VideoDimensionalityState.RotateWAxisParentFirst:
                wAxisParent.transform.rotation = Quaternion.Lerp(startWAxisRotation, firstWAxisRotation, fadingValue);
                return;

            case VideoDimensionalityState.RotateWAxisParentSecond:
                wAxisParent.transform.rotation = Quaternion.Lerp(firstWAxisRotation, secondWAxisRotation, fadingValue);
                return;
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

        fadingText = null;
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
        }

        customText ??= $"<color=\"grey\">" +
            $"(<color=\"red\">{referencePoint.transform.position.x:F2}</color>" +
            $", <color=\"green\">{referencePoint.transform.position.y:F2}</color>" +
            (includeZ ? $", <color=#0080FF>{referencePoint.transform.position.z:F2}</color>" : "") +
            ")";

        referencePointPositionText.text = customText;
    }
}
