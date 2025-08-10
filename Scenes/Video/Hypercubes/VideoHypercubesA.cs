using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum VideoHypercubesAState
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
    HighlightFace,
    HideRest,
    UnhighlightFace,
}

[RequireComponent(typeof(Camera))]
public class VideoHypercubesA : AnimatedStateMachine<VideoHypercubesAState>
{
    [SerializeField]
    private GameObject centerSphereObject;
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
    [SerializeField]
    private GameObject highlightableCubePrefab;

    private readonly List<GameObject> currentHypercubeVertices = new();
    private readonly List<LineRenderer> currentHypercubeLines = new();
    private readonly List<GameObject> unconnectedHypercubeVertices = new();

    private GameObject highlightedFaceObject;

    private Camera cam;

    private readonly Fading _defaultFading = new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    protected override Fading DefaultFading => _defaultFading;
    private readonly Dictionary<VideoHypercubesAState, float> _autoSkipStates = new()
    {
        { VideoHypercubesAState.OrthographicToPerspective, 1f }
    };
    private readonly Dictionary<VideoHypercubesAState, Fading[]> _additionalFadings = new()
    {
        { VideoHypercubesAState.OrthographicToPerspective, new[] { new Fading(0.5f, new Easing(Easing.Type.Expo, Easing.IO.Out)) } },
    };
    protected override Dictionary<VideoHypercubesAState, Fading[]> AdditionalFadings => _additionalFadings;
    protected override Dictionary<VideoHypercubesAState, float> AutoSkipStates => _autoSkipStates;

    

    protected override void OnEnterState(VideoHypercubesAState state)
    {
        switch (state)
        {
            case VideoHypercubesAState.Start:
                OnStart();
                return;

            case VideoHypercubesAState.XAxis:
                xAxisObject.SetActive(true);
                return;

            case VideoHypercubesAState.AddFirstVertex:
                GameObject vertex = Instantiate(vertexObjectPrefab, new Vector3(3, 0, 0), Quaternion.identity);

                vertex.SetActive(true);
                currentHypercubeVertices.Add(vertex);
                return;

            case VideoHypercubesAState.DuplicateVertex:
                DuplicateCurrentHypercube(new Vector3(2, 0, 0), 1f);
                return;

            case VideoHypercubesAState.MakeLine:
                ConnectHypercubes();
                return;

            case VideoHypercubesAState.YAxis:
                yAxisObject.SetActive(true);
                return;

            case VideoHypercubesAState.DuplicateLine:
                DuplicateCurrentHypercube(new Vector3(0, 2, 0), 1f);
                return;

            case VideoHypercubesAState.MakeSquare:
                ConnectHypercubes();
                return;

            case VideoHypercubesAState.ZAxis:
                zAxisObject.SetActive(true);
                return;

            case VideoHypercubesAState.DuplicateSquare:
                DuplicateCurrentHypercube(new Vector3(0, 0, 2), 1f);
                return;

            case VideoHypercubesAState.MakeCube:
                ConnectHypercubes();
                return;

            case VideoHypercubesAState.HighlightFace:
                if (highlightedFaceObject != null)
                {
                    Destroy(highlightedFaceObject);
                }
                highlightedFaceObject = Instantiate(highlightableCubePrefab, new Vector3(4f, 2f, 1f), Quaternion.identity);
                highlightedFaceObject.transform.localScale = new Vector3(2f, 0, 2f);
                highlightedFaceObject.SetActive(true);

                MeshRenderer mr = highlightedFaceObject.GetComponent<MeshRenderer>();
                mr.material = new Material(mr.material);

                FadeSingle(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
                    (fadingValue, isEnter, isExit) =>
                    {
                        Material mat = mr.material;
                        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, fadingValue * .5f);
                    });
                return;

            case VideoHypercubesAState.HideRest:
                Vector3 originalHighlightedFaceObjectPosition = highlightedFaceObject.transform.position;
                Vector3 originalCamPosition = transform.position;

                Vector3 delta = new(20, 0, 0);

                FadeSingle(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.In)),
                    (fadingValue, isEnter, isExit) =>
                    {
                        if (fadingValue < 1f)
                        {
                            highlightedFaceObject.transform.position = Vector3.Lerp(originalHighlightedFaceObjectPosition, originalHighlightedFaceObjectPosition + delta, fadingValue);
                            transform.position = Vector3.Lerp(originalCamPosition, originalCamPosition + delta, fadingValue);
                        }
                        else
                        {
                            // Hide all other vertices and lines
                            foreach (GameObject vertex in currentHypercubeVertices)
                            {
                                vertex.SetActive(false);
                            }
                            foreach (LineRenderer line in currentHypercubeLines)
                            {
                                line.gameObject.SetActive(false);
                            }
                            centerSphereObject.SetActive(false);
                        }
                    });

                return;

            case VideoHypercubesAState.UnhighlightFace:
                MeshRenderer mr1 = highlightedFaceObject.GetComponent<MeshRenderer>();

                FadeSingle(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
                    (fadingValue, isEnter, isExit) =>
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
        }
    }

    protected override void OnExitState(VideoHypercubesAState state)
    {

    }


    protected override void OnUpdateState(VideoHypercubesAState state, float fadingValue, float[] additionalFadingValues)
    {
        switch (state)
        {
            case VideoHypercubesAState.Start:
                return;

            case VideoHypercubesAState.XAxis:
                xAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                xAxisObject.transform.localPosition = new Vector3(fadingValue * 4f, 0, 0);
                return;

            case VideoHypercubesAState.AddFirstVertex:
                currentHypercubeVertices[0].transform.localScale = new Vector3(.0625f, .0625f, .0625f) * fadingValue;
                currentHypercubeVertices[0].transform.localPosition = new Vector3(3, 0, 0);
                return;

            case VideoHypercubesAState.YAxis:
                yAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                yAxisObject.transform.localPosition = new Vector3(0, fadingValue * 4f, 0);
                return;

            case VideoHypercubesAState.ZAxis:
                zAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                zAxisObject.transform.localPosition = new Vector3(0, 0, fadingValue * 4f);
                return;

            case VideoHypercubesAState.OrthographicToPerspective:
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

        FadeSingle(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut)),
            (fadingValue, isEnter, isExit) =>
            {
                parent.transform.position = averagePosition + positionDelta * fadingValue;
                parent.transform.localScale = Vector3.one + (fadingValue * (scaleMultiplier - 1f) * Vector3.one);

                if (isExit)
                {
                    while (parent.transform.childCount > 0)
                    {
                        Transform child = parent.transform.GetChild(0);
                        child.SetParent(null, true);
                    }

                    Destroy(parent);
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

            GameObject obj = currentLine.gameObject;

            FadeSingle(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), (float)index / unconnectedHypercubeVertices.Count),
                (fadingValue, isEnter, isExit) =>
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
        centerSphereObject.SetActive(true);
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

        if (highlightedFaceObject != null)
        {
            Destroy(highlightedFaceObject);
            highlightedFaceObject = null;
        }
    }
}
