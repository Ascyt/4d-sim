using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum VideoHypercubesState
{
    Start,
    XAxis,
    AddFirstVertex,
    DuplicateVertex,
    MakeLine,
    YAxis,
    DuplicateLine,
    MakeSquare,
    OrthographicToPerspective,
    ZAxis,
    DuplicateSquare,
    MakeCube,
    DuplicateCube,
    MakeTesseract,
}

[RequireComponent(typeof(Camera))]
public class VideoHypercubes : AnimatedStateMachine<VideoHypercubesState>
{
    [SerializeField]
    private GameObject xAxisObject;
    [SerializeField]
    private GameObject yAxisObject;
    [SerializeField]
    private GameObject zAxisObject;
    [SerializeField]
    private GameObject vertexObjectPrefab;
    [SerializeField]
    private LineRenderer lineRendererPrefab;

    private readonly List<GameObject> currentHypercubeVertices = new();
    private readonly List<LineRenderer> currentHypercubeLines = new();
    private readonly List<GameObject> unconnectedHypercubeVertices = new();

    private Camera cam;

    private readonly Fading _defaultFading = new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    protected override Fading DefaultFading => _defaultFading;
    private readonly Dictionary<VideoHypercubesState, float> _autoSkipStates = new()
    {
        { VideoHypercubesState.OrthographicToPerspective, 1f }
    };
    private readonly Dictionary<VideoHypercubesState, Fading[]> _additionalFadings = new()
    {
        { VideoHypercubesState.OrthographicToPerspective, new[] { new Fading(0.5f, new Easing(Easing.Type.Expo, Easing.IO.Out)) } },
    };
    protected override Dictionary<VideoHypercubesState, Fading[]> AdditionalFadings => _additionalFadings;
    protected override Dictionary<VideoHypercubesState, float> AutoSkipStates => _autoSkipStates;

    protected override void OnEnterState(VideoHypercubesState state)
    {
        switch (state)
        {
            case VideoHypercubesState.Start:
                OnStart();
                return;

            case VideoHypercubesState.XAxis:
                xAxisObject.SetActive(true);
                return;

            case VideoHypercubesState.AddFirstVertex:
                GameObject vertex = Instantiate(vertexObjectPrefab, new Vector3(3, 0, 0), Quaternion.identity);

                vertex.SetActive(true);
                currentHypercubeVertices.Add(vertex);
                return;

            case VideoHypercubesState.DuplicateVertex:
                DuplicateCurrentHypercube(new Vector3(2, 0, 0), 1f);
                return;

            case VideoHypercubesState.MakeLine:
                ConnectHypercubes();
                return;

            case VideoHypercubesState.YAxis:
                yAxisObject.SetActive(true);
                return;

            case VideoHypercubesState.DuplicateLine:
                DuplicateCurrentHypercube(new Vector3(0, 2, 0), 1f);
                return;

            case VideoHypercubesState.MakeSquare:
                ConnectHypercubes();
                return;

            case VideoHypercubesState.ZAxis:
                zAxisObject.SetActive(true);
                return;

            case VideoHypercubesState.DuplicateSquare:
                DuplicateCurrentHypercube(new Vector3(0, 0, 2), 1f);
                return;

            case VideoHypercubesState.MakeCube:
                ConnectHypercubes();
                return;

            case VideoHypercubesState.DuplicateCube:
                DuplicateCurrentHypercube(new Vector3(0, 0, 0), .5f);
                return;

            case VideoHypercubesState.MakeTesseract:
                ConnectHypercubes();
                return;
        }
    }

    protected override void OnExitState(VideoHypercubesState state)
    {

    }


    protected override void OnUpdateState(VideoHypercubesState state, float fadingValue, float[] additionalFadingValues)
    {
        switch (state)
        {
            case VideoHypercubesState.Start:
                return;

            case VideoHypercubesState.XAxis:
                xAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                xAxisObject.transform.localPosition = new Vector3(fadingValue * 4f, 0, 0);
                return;

            case VideoHypercubesState.AddFirstVertex:
                currentHypercubeVertices[0].transform.localScale = new Vector3(.0625f, .0625f, .0625f) * fadingValue;
                currentHypercubeVertices[0].transform.localPosition = new Vector3(3, 0, 0);
                return;

            case VideoHypercubesState.YAxis:
                yAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                yAxisObject.transform.localPosition = new Vector3(0, fadingValue * 4f, 0);
                return;

            case VideoHypercubesState.ZAxis:
                zAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                zAxisObject.transform.localPosition = new Vector3(0, 0, fadingValue * 4f);
                return;

            case VideoHypercubesState.OrthographicToPerspective:
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -3 * (99f * (1 - additionalFadingValues[0]) + 1));
                cam.fieldOfView = 60f / (99f * (1 - additionalFadingValues[0]) + 1);
                return;
        }
    }

    private void DuplicateCurrentHypercube(Vector3 positionDelta, float scaleMultiplier)
    {
        List<GameObject> objsToMove = new();

        Vector3 total = Vector3.zero;
        int count = 0;
        foreach (GameObject vertex in currentHypercubeVertices)
        {
            GameObject newVertex = Instantiate(vertex, vertex.transform.position, Quaternion.identity);

            unconnectedHypercubeVertices.Add(newVertex);

            objsToMove.Add(newVertex);

            total += newVertex.transform.position;
            count++;
        }
        Vector3 averagePosition = count > 0 ? total / count : Vector3.zero;

        foreach (LineRenderer line in currentHypercubeLines.ToArray())
        {
            LineRenderer newLine = Instantiate(line.gameObject, line.gameObject.transform.position, Quaternion.identity).GetComponent<LineRenderer>();

            currentHypercubeLines.Add(newLine);

            objsToMove.Add(newLine.gameObject);
        }

        GameObject parent = new("TempParent");
        parent.transform.position = averagePosition;
        foreach (GameObject obj in objsToMove)
        {
            obj.transform.SetParent(parent.transform, true);
        }

        FadeSingleObject(parent, false, new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
            (obj, original, fadingValue, isEnter, isExit) =>
            {
                obj.transform.position = averagePosition + positionDelta * fadingValue;
                obj.transform.localScale = Vector3.one + (fadingValue * (scaleMultiplier - 1f) * Vector3.one);

                if (isExit)
                {
                    while (obj.transform.childCount > 0)
                    {
                        Transform child = obj.transform.GetChild(0);
                        child.SetParent(null, true);
                    }

                    Destroy(obj);
                }
            });
    }
    private void ConnectHypercubes()
    {
        List<LineRenderer> lines = new();

        // First positions
        foreach (GameObject vertex in currentHypercubeVertices)
        {
            LineRenderer newLine = Instantiate(lineRendererPrefab.gameObject).GetComponent<LineRenderer>();
            newLine.gameObject.transform.position = vertex.transform.position;
            newLine.positionCount = 2;
            newLine.SetPosition(0, Vector3.zero);

            lines.Add(newLine);
        }
        // Second positions
        int index = 0;
        foreach (GameObject vertex in unconnectedHypercubeVertices)
        {
            Vector3 secondPosition = vertex.transform.position;
            LineRenderer currentLine = lines[index];

            FadeSingleObject(currentLine.gameObject, false, new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), (float)index / unconnectedHypercubeVertices.Count),
                (obj, original, fadingValue, isEnter, isExit) =>
                {
                    LineRenderer lr = obj.GetComponent<LineRenderer>();

                    currentLine.SetPosition(1, Vector3.Lerp(Vector3.zero, secondPosition - lr.transform.position, fadingValue));
                });

            index++;
        }

        currentHypercubeLines.AddRange(lines);
        currentHypercubeVertices.AddRange(unconnectedHypercubeVertices);
        unconnectedHypercubeVertices.Clear();
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

        vertexObjectPrefab.SetActive(false);

        cam.transform.SetPositionAndRotation(new Vector3(2.5f, 1, -3 * 100f), Quaternion.identity);
        cam.fieldOfView = 60f / 100f;

        foreach (GameObject obj in currentHypercubeVertices)
        {
            Destroy(obj);
        }
        currentHypercubeVertices.Clear();
        foreach (LineRenderer lr in currentHypercubeLines)
        {
            Destroy(lr.gameObject);
        }
        currentHypercubeLines.Clear();
        foreach (GameObject obj in unconnectedHypercubeVertices)
        {
            Destroy(obj);
        }
        unconnectedHypercubeVertices.Clear();
    }
}
