using System.Collections.Generic;
using UnityEngine;

public enum VideoHypercubesBState
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
    ActualStart,
    DuplicateSquare,
    MakeCube,
    HighlightBackFace,
    UnhighlightBackFace,
    HighlightFrontFace,
    UnhighlightFrontFace,
    DuplicateCube,
    MakeTesseract,
}

[RequireComponent(typeof(Camera))]
public class VideoHypercubesB : AnimatedStateMachine<VideoHypercubesBState>
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

    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    private readonly Dictionary<VideoHypercubesBState, float> _autoSkipStates = new()
    {
        { VideoHypercubesBState.Start, 0f },
        { VideoHypercubesBState.XAxis, 0f },
        { VideoHypercubesBState.AddFirstVertex, 0f },
        { VideoHypercubesBState.DuplicateVertex, 0f },
        { VideoHypercubesBState.MakeLine, 0f },
        { VideoHypercubesBState.YAxis, 0f },
        { VideoHypercubesBState.DuplicateLine, 0f },
        { VideoHypercubesBState.MakeSquare, 0f },
        { VideoHypercubesBState.OrthographicToPerspective, 0f },
        { VideoHypercubesBState.ZAxis, 0f },

        { VideoHypercubesBState.UnhighlightBackFace, 1f }
    };

    protected override Dictionary<VideoHypercubesBState, float> AutoSkipStates => _autoSkipStates;


    protected override void OnEnterState(VideoHypercubesBState state)
    {
        switch (state)
        {
            case VideoHypercubesBState.Start:
                OnStart();
                return;

            case VideoHypercubesBState.XAxis:
                xAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        xAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        xAxisObject.transform.localPosition = new Vector3(fadingValue * 4f, 0, 0);
                    });

                return;

            case VideoHypercubesBState.AddFirstVertex:
                GameObject vertex = Instantiate(vertexObjectPrefab, new Vector3(3, 0, 0), Quaternion.identity);

                vertex.SetActive(true);
                currentHypercubeVertices.Add(vertex);
                currentHypercubeVertices[0].transform.localPosition = new Vector3(3, 0, 0);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        currentHypercubeVertices[0].transform.localScale = new Vector3(.0625f, .0625f, .0625f) * fadingValue;
                    });
                return;

            case VideoHypercubesBState.DuplicateVertex:
                DuplicateCurrentHypercube(new Vector3(2, 0, 0), 1f);
                return;

            case VideoHypercubesBState.MakeLine:
                ConnectHypercubes();
                return;

            case VideoHypercubesBState.YAxis:
                yAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        yAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        yAxisObject.transform.localPosition = new Vector3(0, fadingValue * 4f, 0);
                    });
                return;

            case VideoHypercubesBState.DuplicateLine:
                DuplicateCurrentHypercube(new Vector3(0, 2, 0), 1f);
                return;

            case VideoHypercubesBState.MakeSquare:
                ConnectHypercubes();
                return;

            case VideoHypercubesBState.OrthographicToPerspective:
                Fade(new Fading(0.5f, new Easing(Easing.Type.Expo, Easing.IO.Out)),
                    (fadingValue, isExit) =>
                    {
                        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -3 * (99f * (1 - fadingValue) + 1));
                        cam.fieldOfView = 60f / (99f * (1 - fadingValue) + 1);
                    });
                return;

            case VideoHypercubesBState.ZAxis:
                zAxisObject.SetActive(true);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        zAxisObject.transform.localScale = new Vector3(0.5f, fadingValue * 4f, 0.5f);
                        zAxisObject.transform.localPosition = new Vector3(0, 0, fadingValue * 4f);
                    });
                return;

            case VideoHypercubesBState.ActualStart:
                transform.position = new Vector3(4f, 1f, -3f);
                centerSphereObject.SetActive(false);
                return;

            case VideoHypercubesBState.DuplicateSquare:
                DuplicateCurrentHypercube(new Vector3(0, 0, 2), 1f);
                return;

            case VideoHypercubesBState.MakeCube:
                ConnectHypercubes();
                return;

            case VideoHypercubesBState.HighlightBackFace:
                if (highlightedFaceObject != null)
                {
                    Destroy(highlightedFaceObject);
                }
                highlightedFaceObject = Instantiate(highlightableCubePrefab, new Vector3(4f, 1f, 2f), Quaternion.identity);
                highlightedFaceObject.transform.localScale = new Vector3(2f, 2f, 0f);
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

            case VideoHypercubesBState.UnhighlightBackFace:
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

            case VideoHypercubesBState.HighlightFrontFace:
                highlightedFaceObject = Instantiate(highlightableCubePrefab, new Vector3(4f, 1f, 0f), Quaternion.identity);
                highlightedFaceObject.transform.localScale = new Vector3(2f, 2f, 0f);

                MeshRenderer mr2 = highlightedFaceObject.GetComponent<MeshRenderer>();
                mr2.material = new Material(mr2.material);

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        Material mat = mr2.material;
                        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, fadingValue * .5f);
                    });

                return;

            case VideoHypercubesBState.UnhighlightFrontFace:
                MeshRenderer mr3 = highlightedFaceObject.GetComponent<MeshRenderer>();

                Fade(DefaultFading,
                    (fadingValue, isExit) =>
                    {
                        Material mat = mr3.material;
                        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, (1f - fadingValue) * .5f);

                        if (isExit)
                        {
                            Destroy(mr3.material);
                            Destroy(highlightedFaceObject);
                        }
                    });

                return;

            case VideoHypercubesBState.DuplicateCube:
                DuplicateCurrentHypercube(new Vector3(0, 0, 0), .5f);
                return;

            case VideoHypercubesBState.MakeTesseract:
                ConnectHypercubes();
                return;
        }
    }

    protected override void BeforeExitState(VideoHypercubesBState state)
    {

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

        Fade(DefaultFading,
            (fadingValue, isExit) =>
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

            Fade(new Fading(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut), (float)index / unconnectedHypercubeVertices.Count),
                (fadingValue, isExit) =>
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
